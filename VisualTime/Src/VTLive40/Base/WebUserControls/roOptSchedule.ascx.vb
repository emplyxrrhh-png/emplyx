Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserControls_roOptSchedule
    Inherits UserControlBase

    'Language Tags
    Private CriteriaEquals As String

    Private CriteriaDifferent As String
    Private CriteriaStartsWith As String
    Private CriteriaContains As String
    Private CriteriaNoContains As String
    Private CriteriaMajor As String
    Private CriteriaMajorOrEquals As String
    Private CriteriaMinor As String
    Private CriteriaMinorOrEquals As String
    Private CriteriaTheValue As String
    Private CriteriaTheDate As String
    Private CriteriaTheDateOfJustification As String
    Private CriteriaTheTime As String
    Private CriteriaTheTimeOfJustification As String
    Private CriteriaTheValues As String
    Private CriteriaThePeriod As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Me.optDiary.Attributes("onclick") = "optSchedule_changeTab(0,'" & Me.ClientID & "');"
            Me.optWeekly.Attributes("onclick") = "optSchedule_changeTab(1,'" & Me.ClientID & "');"
            Me.optMonthly.Attributes("onclick") = "optSchedule_changeTab(2,'" & Me.ClientID & "');"
            Me.optAnnual.Attributes("onclick") = "optSchedule_changeTab(3,'" & Me.ClientID & "');"
            Me.opMonth1.Attributes("onchange") = "optSchedule_changeMonth(1,'" & Me.ClientID & "');"
            Me.opMonth2.Attributes("onchange") = "optSchedule_changeMonth(2,'" & Me.ClientID & "');"
            'Me.anualFixDay.Attributes("onchange") = "optSchedule_changeAnual(1,'" & Me.ClientID & "');"
            'Me.anualLastDay.Attributes("onchange") = "optSchedule_changeAnual(2,'" & Me.ClientID & "');"

            Me.cmbM1O1.ClientInstanceName = Me.ClientID & "_cmbM101"
            Me.cmbM1O2.ClientInstanceName = Me.ClientID & "_cmbM102"
            Me.cmbM2O1.ClientInstanceName = Me.ClientID & "_cmbM201"
            Me.cmbM2O2.ClientInstanceName = Me.ClientID & "_cmbM202"
            Me.cmbM2O3.ClientInstanceName = Me.ClientID & "_cmbM203"

            Me.cmbA1O1.ClientInstanceName = Me.ClientID & "_cmbA101"
            Me.cmbA1O2.ClientInstanceName = Me.ClientID & "_cmbA102"

            IsScriptManagerInParent()
            If Not Me.IsPostBack Then
                LoadCombos()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadCombos()
        Try
            Me.cmbWeeklyTo.Items.Clear()
            Me.cmbM2O2.Items.Clear()
            Me.cmbM1O1.Items.Clear()
            Me.cmbM2O3.Items.Clear()
            Me.cmbA1O1.Items.Clear()
            Me.cmbM1O2.Items.Clear()
            Me.cmbA1O2.Items.Clear()
            Me.cmbM2O1.Items.Clear()

            Me.cmbWeeklyTo.ValueType = GetType(Integer)
            Me.cmbM2O2.ValueType = GetType(Integer)
            Me.cmbM1O1.ValueType = GetType(Integer)
            Me.cmbM2O3.ValueType = GetType(Integer)
            Me.cmbA1O1.ValueType = GetType(Integer)
            Me.cmbM1O2.ValueType = GetType(Integer)
            Me.cmbA1O2.ValueType = GetType(Integer)
            Me.cmbM2O1.ValueType = GetType(Integer)

            For n As Integer = 1 To 7
                Me.cmbWeeklyTo.Items.Add(Me.Language.Keyword("weekday." & n.ToString), n)
                Me.cmbM2O2.Items.Add(Me.Language.Keyword("weekday." & n.ToString), n)
            Next

            cmbWeeklyTo.SelectedIndex = 0
            cmbM2O2.SelectedIndex = 0

            For nDays As Integer = 1 To 31
                Me.cmbM1O1.Items.Add(nDays.ToString, nDays)
                Me.cmbM2O3.Items.Add(nDays.ToString, nDays)
                Me.cmbA1O1.Items.Add(nDays.ToString, nDays)
            Next
            Me.cmbM1O1.Items.Add(Me.Language.Keyword("Last"), "32")

            cmbM1O1.SelectedIndex = 0
            cmbM2O3.SelectedIndex = 0
            cmbA1O1.SelectedIndex = 0

            For nMonths As Integer = 1 To 11
                Me.cmbM1O2.Items.Add(nMonths.ToString, nMonths)
                Me.cmbA1O2.Items.Add(nMonths.ToString, nMonths)
            Next
            Me.cmbA1O2.Items.Add("12", 12)

            cmbM1O2.SelectedIndex = 0
            cmbA1O2.SelectedIndex = 0

            For nNums As Integer = 1 To 4
                Me.cmbM2O1.Items.Add(Me.Language.Keyword("num." & nNums.ToString), nNums)
            Next
            Me.cmbM2O1.Items.Add(Me.Language.Keyword("num.Last"), 5)

            cmbM2O1.SelectedIndex = 0
        Catch e As Exception
            Response.Write(e.Message & " " & e.StackTrace)
        End Try
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roOptSchedule", "~/Base/Scripts/roOptSchedule.js", Me.Parent.Page)

        Return True
    End Function

    Public Sub SetValue(ByVal schedule As roLabAgreeSchedule)

        Me.optDiary.Checked = False
        Me.optWeekly.Checked = False
        Me.optMonthly.Checked = False
        Me.optAnnual.Checked = False
        Me.opMonth1.Checked = False
        Me.opMonth2.Checked = False

        Me.divAnnual.Style("display") = "none"
        Me.divMonthly.Style("display") = "none"
        Me.divWeekly.Style("display") = "none"
        Me.divDaily.Style("display") = "none"

        If schedule IsNot Nothing Then
            Select Case schedule.ScheduleType
                Case LabAgreeScheduleScheduleType.Daily
                    Me.divDaily.Style("display") = ""
                    Me.optDiary.Checked = True
                    Me.txtDaily.Text = schedule.Days
                Case LabAgreeScheduleScheduleType.Weekly
                    Me.divWeekly.Style("display") = ""
                    Me.optWeekly.Checked = True
                    Me.cmbWeeklyTo.SelectedItem = Me.cmbWeeklyTo.Items.FindByValue(CInt(schedule.WeekDay))
                Case LabAgreeScheduleScheduleType.Monthly
                    Me.divMonthly.Style("display") = ""
                    Me.optMonthly.Checked = True
                    Select Case schedule.MonthlyType
                        Case LabAgreeScheduleMonthlyType.DayAndMonth
                            Me.opMonth1.Checked = True
                            Me.cmbM1O1.SelectedItem = Me.cmbM1O1.Items.FindByValue(schedule.Day)
                            Me.cmbM1O2.SelectedItem = Me.cmbM1O2.Items.FindByValue(schedule.Months)
                        Case LabAgreeScheduleMonthlyType.DayAndStartup
                            Me.opMonth2.Checked = True
                            Me.cmbM2O1.SelectedItem = Me.cmbM2O1.Items.FindByValue(schedule.Start)
                            Me.cmbM2O2.SelectedItem = Me.cmbM2O2.Items.FindByValue(CInt(schedule.WeekDay))
                            Me.cmbM2O3.SelectedItem = Me.cmbM2O3.Items.FindByValue(schedule.Months)
                    End Select
                Case LabAgreeScheduleScheduleType.Annual
                    Me.divAnnual.Style("display") = ""
                    Me.optAnnual.Checked = True

                    Me.cmbA1O1.SelectedItem = Me.cmbA1O1.Items.FindByValue(schedule.Day)
                    Me.cmbA1O2.SelectedItem = Me.cmbA1O2.Items.FindByValue(schedule.Month)
            End Select
        Else
            Me.optDiary.Checked = True
            Me.txtDaily.Text = 1
            Me.divDaily.Style("display") = ""
        End If

        If Not Me.opMonth1.Checked AndAlso Not Me.opMonth2.Checked Then
            Me.opMonth1.Checked = True
        End If

    End Sub

    Public Function GetValue() As roLabAgreeSchedule
        Dim schedule As New roLabAgreeSchedule

        If Me.optDiary.Checked Then
            schedule.ScheduleType = LabAgreeScheduleScheduleType.Daily
            schedule.Days = roTypes.Any2Integer(txtDaily.Text)
        ElseIf Me.optWeekly.Checked Then
            schedule.ScheduleType = LabAgreeScheduleScheduleType.Weekly
            schedule.WeekDay = (roTypes.Any2Integer(Me.cmbWeeklyTo.SelectedItem.Value))
        ElseIf Me.optMonthly.Checked Then
            schedule.ScheduleType = LabAgreeScheduleScheduleType.Monthly
            If Me.opMonth1.Checked Then
                schedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth
                schedule.Day = Me.cmbM1O1.SelectedItem.Value
                schedule.Months = Me.cmbM1O2.SelectedItem.Value
            Else
                schedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup
                schedule.Start = Me.cmbM2O1.SelectedItem.Value
                schedule.WeekDay = (roTypes.Any2Integer(Me.cmbM2O2.SelectedItem.Value))
                schedule.Months = Me.cmbM2O3.SelectedItem.Value
            End If
        ElseIf Me.optAnnual.Checked Then
            schedule.ScheduleType = LabAgreeScheduleScheduleType.Annual

            schedule.Day = Me.cmbA1O1.SelectedItem.Value
            schedule.Month = Me.cmbA1O2.SelectedItem.Value

        End If

        ' La enumeración de días de la semana empieza por 1, y no admite el 0
        If schedule.WeekDay = 0 Then schedule.WeekDay = 1

        Return schedule
    End Function

End Class