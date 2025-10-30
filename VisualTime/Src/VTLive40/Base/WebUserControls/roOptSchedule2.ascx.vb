Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserControls_roOptSchedule2
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
            IsScriptManagerInParent()
            LoadCombos()

            optHours.Attributes("onclick") = "optSchedule2_changeTab(0,'" & Me.ClientID & "');"
            optDiary.Attributes("onclick") = "optSchedule2_changeTab(1,'" & Me.ClientID & "');"
            optWeekly.Attributes("onclick") = "optSchedule2_changeTab(2,'" & Me.ClientID & "');"
            optMonthly.Attributes("onclick") = "optSchedule2_changeTab(3,'" & Me.ClientID & "');"
            optOneTime.Attributes("onclick") = "optSchedule2_changeTab(4,'" & Me.ClientID & "');"

            opMonth1.Attributes("onclick") = "optSchedule2_changeMonth(1,'" & Me.ClientID & "');"
            opMonth2.Attributes("onclick") = "optSchedule2_changeMonth(2,'" & Me.ClientID & "');"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadCombos()
        Try
            Me.cmbDays.Items.Clear()
            For n As Integer = 1 To 366
                Me.cmbDays.Items.Add(New DevExpress.Web.ListEditItem(n.ToString, n.ToString))

            Next
            Me.cmbWeeklyTo.Items.Clear()
            For n As Integer = 1 To 100
                Me.cmbWeeklyTo.Items.Add(New DevExpress.Web.ListEditItem(n.ToString, n.ToString))
            Next

            Me.cmbM2O2.Items.Clear()
            For n As Integer = 1 To 7
                Me.cmbM2O2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Keyword("weekday." & n.ToString), n.ToString))
            Next

            Me.cmbM1O1.Items.Clear()
            For nDays As Integer = 1 To 31
                Me.cmbM1O1.Items.Add(New DevExpress.Web.ListEditItem(nDays.ToString, nDays))
            Next

            Me.cmbM2O1.Items.Clear()
            For nNums As Integer = 1 To 4
                Me.cmbM2O1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Keyword("num." & nNums.ToString), nNums))
            Next
            Me.cmbM2O1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Keyword("num.Last"), "5"))
        Catch e As Exception
            Response.Write(e.Message & " " & e.StackTrace)
        End Try
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roOptSchedule2", "~/Base/Scripts/roOptSchedule2.js", Me.Parent.Page)

        Return True
    End Function

    Public Function GetValues(ByRef oScheduler As roReportSchedulerSchedule) As Boolean

        Dim bolret As Boolean = False
        Try
            If Not txtHours.Value = Nothing Then
                oScheduler.Hour = roTypes.Any2Time(Me.txtHours.Value).TimeOnly
            Else
                oScheduler.Hour = roTypes.Any2Time("00:00").TimeOnly
            End If

            If Me.optHours.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.Hours

                If Not txtHoursSchedule.Value = Nothing Then
                    oScheduler.Hour = roTypes.Any2Time(Me.txtHoursSchedule.Value).TimeOnly
                Else
                    oScheduler.Hour = roTypes.Any2Time("00:00").TimeOnly
                End If
            End If

            If Me.optOneTime.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.OneTime
                oScheduler.DateSchedule = txtDateSchedule.Date
            End If

            If Me.optMonthly.Checked Then
                If Not opMonth1.Checked And Not opMonth2.Checked Then
                    opMonth1.Checked = True
                End If
            End If
            If Me.optMonthly.Checked And opMonth1.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.Monthly
                oScheduler.MonthlyType = eRSMonthlyType.DayOfMonth
                If Not IsNothing(cmbM1O1.SelectedItem) Then
                    oScheduler.Day = roTypes.Any2Integer(cmbM1O1.SelectedItem.Value)
                Else
                    oScheduler.Day = 1
                End If
            End If
            If Me.optMonthly.Checked And opMonth2.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.Monthly
                oScheduler.MonthlyType = eRSMonthlyType.StartAndDayMonth

                If Not IsNothing(cmbM2O1.SelectedItem) Then
                    oScheduler.Start = roTypes.Any2Integer(cmbM2O1.SelectedItem.Value)
                Else
                    oScheduler.Start = 1
                End If

            End If
            If Me.optMonthly.Checked And opMonth2.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.Monthly
                oScheduler.MonthlyType = eRSMonthlyType.StartAndDayMonth
                If Not IsNothing(cmbM2O2.SelectedItem) Then
                    oScheduler.WeekDay = roTypes.Any2Integer(cmbM2O2.SelectedItem.Value) ' - 1
                Else
                    oScheduler.WeekDay = eRSWeekDay.Monday
                End If

            End If

            If Me.optWeekly.Checked Then
                Dim strWeekDays As String = "0000000"
                Dim strWeekDays1 As String = "0"
                Dim strWeekDays2 As String = "0"
                Dim strWeekDays3 As String = "0"
                Dim strWeekDays4 As String = "0"
                Dim strWeekDays5 As String = "0"
                Dim strWeekDays6 As String = "0"
                Dim strWeekDays7 As String = "0"

                oScheduler.ScheduleType = eRSScheduleType.Weekly
                If Not IsNothing(cmbWeeklyTo.SelectedItem) Then
                    oScheduler.Weeks = roTypes.Any2Integer(cmbWeeklyTo.SelectedItem.Value)
                Else
                    oScheduler.Weeks = 1
                End If

                If chkWeekDay1.Checked Then
                    strWeekDays1 = "1"
                End If
                If chkWeekDay2.Checked Then
                    strWeekDays2 = "1"
                End If
                If chkWeekDay3.Checked Then
                    strWeekDays3 = "1"
                End If
                If chkWeekDay4.Checked Then
                    strWeekDays4 = "1"
                End If
                If chkWeekDay5.Checked Then
                    strWeekDays5 = "1"
                End If
                If chkWeekDay6.Checked Then
                    strWeekDays6 = "1"
                End If
                If chkWeekDay7.Checked Then
                    strWeekDays7 = "1"
                End If
                strWeekDays = strWeekDays1 & strWeekDays2 & strWeekDays3 & strWeekDays4 & strWeekDays5 & strWeekDays6 & strWeekDays7
                oScheduler.WeekDays = strWeekDays
            Else
                oScheduler.WeekDays = "0000000"
            End If

            If Me.optDiary.Checked Then
                oScheduler.ScheduleType = eRSScheduleType.Daily
                If Not IsNothing(cmbDays.SelectedItem) Then
                    oScheduler.Days = roTypes.Any2Integer(cmbDays.SelectedItem.Value)
                Else
                    oScheduler.Days = 1
                End If

            End If

            bolret = True
        Catch ex As Exception

        End Try

        Return bolret

    End Function

    Public Function LoadValues(ByVal oScheduler As Robotics.Base.DTOs.roReportSchedulerSchedule, ByVal bolDisabled As Boolean) As Boolean
        Dim boloptHours As Boolean = False
        Dim boloptDiary As Boolean = False
        Dim boloptWeekly As Boolean = False
        Dim boloptMonthly As Boolean = False
        Dim boloptOneTime As Boolean = False

        Dim txtDaily As Integer = 0
        Dim txtWeeks As Integer = 0
        Dim txtWeekDays As String = ""

        Dim boloptMonth1 As Boolean = False 'el dia
        Dim txtM1A1 As Integer = 0 'dia de cada

        Dim boloptMonth2 As Boolean = False 'el
        Dim txtM2A1 As Integer = 0  'primer
        Dim txtM2A2 As Integer = 0  'lunes de cada

        Dim txtA1A1 As String = "" 'Dia
        Dim txtA1A2 As String = "" 'Mes

        Dim txtWeekDay1 As Boolean = False
        Dim txtWeekDay2 As Boolean = False
        Dim txtWeekDay3 As Boolean = False
        Dim txtWeekDay4 As Boolean = False
        Dim txtWeekDay5 As Boolean = False
        Dim txtWeekDay6 As Boolean = False
        Dim txtWeekDay7 As Boolean = False

        Dim txtHours As String = "00:00"
        Dim txtHoursSchedule As String = "00:00"

        If oScheduler IsNot Nothing Then
            Select Case oScheduler.ScheduleType
                Case eRSScheduleType.Hours
                    boloptHours = True
                    txtHoursSchedule = oScheduler.Hour
                Case eRSScheduleType.Daily
                    boloptDiary = True
                    txtDaily = oScheduler.Days
                Case eRSScheduleType.Weekly
                    boloptWeekly = True
                    txtWeeks = oScheduler.Weeks
                    txtWeekDays = oScheduler.WeekDays
                    If txtWeekDays <> "" Then
                        If txtWeekDays.Substring(0, 1) = "1" Then txtWeekDay1 = True
                        If txtWeekDays.Substring(1, 1) = "1" Then txtWeekDay2 = True
                        If txtWeekDays.Substring(2, 1) = "1" Then txtWeekDay3 = True
                        If txtWeekDays.Substring(3, 1) = "1" Then txtWeekDay4 = True
                        If txtWeekDays.Substring(4, 1) = "1" Then txtWeekDay5 = True
                        If txtWeekDays.Substring(5, 1) = "1" Then txtWeekDay6 = True
                        If txtWeekDays.Substring(6, 1) = "1" Then txtWeekDay7 = True
                    End If
                Case eRSScheduleType.Monthly
                    boloptMonthly = True
                    Select Case oScheduler.MonthlyType
                        Case eRSMonthlyType.DayOfMonth
                            boloptMonth1 = True
                            txtM1A1 = oScheduler.Day
                        Case eRSMonthlyType.StartAndDayMonth
                            boloptMonth2 = True
                            txtM2A1 = oScheduler.Start
                            txtM2A2 = oScheduler.WeekDay ' + 1
                    End Select
                Case eRSScheduleType.OneTime
                    boloptOneTime = True
                    txtA1A1 = Format(oScheduler.DateSchedule, HelperWeb.GetShortDateFormat)
            End Select
            txtHours = oScheduler.Hour
        Else
            boloptDiary = True
            txtDaily = 1
        End If

        'oJSONFields.Add(New JSONFieldItem("optDiary", boloptDiary.ToString.ToLower, New String() {"gBox1_optSchedule1_optDiary"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.optDiary.Checked = boloptDiary

        'oJSONFields.Add(New JSONFieldItem("optWeekly", boloptWeekly.ToString.ToLower, New String() {"gBox1_optSchedule1_optWeekly"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.optWeekly.Checked = boloptWeekly

        'oJSONFields.Add(New JSONFieldItem("optMonthly", boloptMonthly.ToString.ToLower, New String() {"gBox1_optSchedule1_optMonthly"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.optMonthly.Checked = boloptMonthly

        'oJSONFields.Add(New JSONFieldItem("optOneTime", boloptOneTime.ToString.ToLower, New String() {"gBox1_optSchedule1_optOneTime"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.optOneTime.Checked = boloptOneTime

        Me.optHours.Checked = boloptHours

        'Horas
        If oScheduler IsNot Nothing Then
            'Hora
            'oJSONFields.Add(New JSONFieldItem("ScheduleHours", txtHours, New String() {"gBox1_optSchedule1_txtHours"}, roJSON.JSONType.Time_JSON, Nothing, oDisable))
            Me.txtHoursSchedule.DateTime = roTypes.Any2Time(oScheduler.Hour).TimeOnly
        Else
            Me.txtHoursSchedule.DateTime = roTypes.Any2Time("00:00").TimeOnly
        End If

        'Diari
        Me.cmbDays.SelectedItem = Me.cmbDays.Items.FindByValue(txtDaily.ToString)
        If Me.cmbDays.SelectedItem Is Nothing Then Me.cmbDays.SelectedIndex = 0
        Me.cmbDays.Enabled = Not bolDisabled

        'Semanal
        Me.cmbWeeklyTo.SelectedItem = Me.cmbWeeklyTo.Items.FindByValue(txtWeeks.ToString)
        If Me.cmbWeeklyTo.SelectedItem Is Nothing Then Me.cmbWeeklyTo.SelectedIndex = 0
        Me.cmbWeeklyTo.Enabled = Not bolDisabled

        Me.chkWeekDay1.Checked = txtWeekDay1
        Me.chkWeekDay2.Checked = txtWeekDay2
        Me.chkWeekDay3.Checked = txtWeekDay3
        Me.chkWeekDay4.Checked = txtWeekDay4
        Me.chkWeekDay5.Checked = txtWeekDay5
        Me.chkWeekDay6.Checked = txtWeekDay6
        Me.chkWeekDay7.Checked = txtWeekDay7

        'Mensual
        '1
        'oJSONFields.Add(New JSONFieldItem("optMonth1", boloptMonth1.ToString.ToLower, New String() {"gBox1_optSchedule1_opMonth1"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.opMonth1.Checked = boloptMonth1

        'oJSONFields.Add(New JSONFieldItem("cmbM1O1", txtM1A1.ToString, New String() {"gBox1_optSchedule1_cmbM1O1", "gBox1_optSchedule1_cmbM1O1_Text", "gBox1_optSchedule1_cmbM1O1_Value"}, roJSON.JSONType.ComboBox_JSON, Nothing, oDisable))
        Me.cmbM1O1.SelectedItem = Me.cmbM1O1.Items.FindByValue(txtM1A1.ToString)
        If Me.cmbM1O1.SelectedItem Is Nothing Then Me.cmbM1O1.SelectedIndex = 0
        Me.cmbM1O1.Enabled = Not bolDisabled

        '2
        'oJSONFields.Add(New JSONFieldItem("optMonth2", boloptMonth2.ToString.ToLower, New String() {"gBox1_optSchedule1_opMonth2"}, roJSON.JSONType.Radio_JSON, Nothing, oDisable))
        Me.opMonth2.Checked = boloptMonth2

        'oJSONFields.Add(New JSONFieldItem("cmbM2O1", txtM2A1.ToString, New String() {"gBox1_optSchedule1_cmbM2O1", "gBox1_optSchedule1_cmbM2O1_Text", "gBox1_optSchedule1_cmbM2O1_Value"}, roJSON.JSONType.ComboBox_JSON, Nothing, oDisable))
        Me.cmbM2O1.SelectedItem = Me.cmbM2O1.Items.FindByValue(txtM2A1.ToString)
        If Me.cmbM2O1.SelectedItem Is Nothing Then Me.cmbM2O1.SelectedIndex = 0
        Me.cmbM2O1.Enabled = Not bolDisabled

        'oJSONFields.Add(New JSONFieldItem("cmbM2O2", txtM2A2.ToString, New String() {"gBox1_optSchedule1_cmbM2O2", "gBox1_optSchedule1_cmbM2O2_Text", "gBox1_optSchedule1_cmbM2O2_Value"}, roJSON.JSONType.ComboBox_JSON, Nothing, oDisable))
        Me.cmbM2O2.SelectedItem = Me.cmbM2O2.Items.FindByValue(txtM2A2.ToString)
        If Me.cmbM2O2.SelectedItem Is Nothing Then Me.cmbM2O2.SelectedIndex = 0
        Me.cmbM2O2.Enabled = Not bolDisabled

        'Anual
        'oJSONFields.Add(New JSONFieldItem("cmbA1O1", txtA1A1.ToString, New String() {"gBox1_optSchedule1_txtDateSchedule"}, roJSON.JSONType.Date_JSON, Nothing, oDisable))
        If oScheduler IsNot Nothing Then
            If oScheduler.DateSchedule = Date.MinValue Then
                txtDateSchedule.Date = DateTime.Now.Date.AddDays(1)
            Else
                txtDateSchedule.Date = oScheduler.DateSchedule
            End If

            'Hora
            'oJSONFields.Add(New JSONFieldItem("ScheduleHours", txtHours, New String() {"gBox1_optSchedule1_txtHours"}, roJSON.JSONType.Time_JSON, Nothing, oDisable))
            Me.txtHours.DateTime = roTypes.Any2Time(oScheduler.Hour).TimeOnly
        Else
            txtDateSchedule.Date = DateTime.Now.Date.AddDays(1)
            Me.txtHours.DateTime = roTypes.Any2Time("00:00").TimeOnly
        End If

        Me.txtHours.Enabled = Not bolDisabled
        Me.txtDateSchedule.Enabled = Not bolDisabled

        SetVisibility(boloptHours, boloptDiary, boloptWeekly, boloptMonthly, boloptOneTime)

        Return True

    End Function

    Private Sub SetVisibility(ByVal boloptHours As Boolean, ByVal boloptDiary As Boolean, ByVal boloptWeekly As Boolean, ByVal boloptMonthly As Boolean, ByVal boloptOneTime As Boolean)
        Me.divHours.Style("display") = IIf(boloptHours, "", "none")
        Me.divDaily.Style("display") = IIf(boloptDiary, "", "none")
        Me.divWeekly.Style("display") = IIf(boloptWeekly, "", "none")
        Me.divMonthly.Style("display") = IIf(boloptMonthly, "", "none")
        Me.divOneTime.Style("display") = IIf(boloptOneTime, "", "none")

        Me.divCommonHours.Style("display") = IIf(boloptHours, "none", "")

    End Sub

End Class