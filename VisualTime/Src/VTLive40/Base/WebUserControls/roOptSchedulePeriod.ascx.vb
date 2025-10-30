Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class WebUserControls_roOptSchedulePeriod
    Inherits UserControlBase

    Public Sub New()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            IsScriptManagerInParent()
            LoadCombos()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Initialize()

        rblPeriod.ValueType = GetType(Integer)
        rblPeriod.Items.Clear()

        For Each eType As TypePeriodEnum In System.Enum.GetValues(GetType(TypePeriodEnum))
            rblPeriod.Items.Add(Language.Translate("Scope." & eType.ToString, DefaultScope), CInt(eType))
        Next

    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase

        Return True
    End Function

    Private Sub LoadCombos()
        Try
            Me.cmbMonthsShiftedDay.Items.Clear()
            For n As Integer = 1 To 28
                Me.cmbMonthsShiftedDay.Items.Add(New DevExpress.Web.ListEditItem(n.ToString, n.ToString))

            Next
            Me.cmbMonthsShifted.Items.Clear()
            For n As Integer = 1 To 12
                Me.cmbMonthsShifted.Items.Add(New DevExpress.Web.ListEditItem(n.ToString, n.ToString))

            Next
        Catch e As Exception
            Response.Write(e.Message & " " & e.StackTrace)
        End Try
    End Sub

    Public Function GetValues(ByRef eType As TypePeriodEnum, ByRef xBeginDate As DateTime, ByRef xEndDate As DateTime, ByRef iShiftedMonthStartDay As Integer, ByRef iShiftedMonthMonthsInAdvance As Integer) As Boolean

        Dim bolret As Boolean = False
        Try
            If rblPeriod.SelectedItem IsNot Nothing Then
                eType = System.Enum.Parse(GetType(TypePeriodEnum), rblPeriod.SelectedItem.Value)

                Dim dtMonth As Date

                If eType <> TypePeriodEnum.PeriodNMonthsAgoFromDay Then
                    cmbMonthsShiftedDay.ClientEnabled = False
                    cmbMonthsShifted.ClientEnabled = False
                Else
                    cmbMonthsShiftedDay.ClientEnabled = True
                    cmbMonthsShifted.ClientEnabled = True
                End If

                Select Case eType
                    Case TypePeriodEnum.PeriodToday
                        Me.txtBeginDate.Date = Now
                        Me.txtEndDate.Date = Now
                    Case TypePeriodEnum.PeriodTomorrow
                        Me.txtBeginDate.Date = Now.AddDays(1)
                        Me.txtEndDate.Date = Now.AddDays(1)
                    Case TypePeriodEnum.PeriodYesterday
                        Me.txtBeginDate.Date = Now.AddDays(-1)
                        Me.txtEndDate.Value = Now.AddDays(-1)
                    Case TypePeriodEnum.PeriodCurrentWeek
                        Me.txtBeginDate.Date = Now.AddDays(1 - Weekday(Now, vbMonday))
                        Me.txtEndDate.Value = Now.AddDays(7 - Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodLastWeek
                        Me.txtBeginDate.Date = Now.AddDays(-6 - Weekday(Now, vbMonday))
                        Me.txtEndDate.Value = Now.AddDays(-Weekday(Now, vbMonday))
                    Case TypePeriodEnum.PeriodCurrentMonth
                        Me.txtBeginDate.Date = New Date(Now.Year, Now.Month, 1)
                        Me.txtEndDate.Value = New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day)
                    Case TypePeriodEnum.PeriodNextWeek
                        'Semana siguiente
                        Dim today As DateTime = DateTime.Today
                        Dim daysUntilMonday As Integer = (CInt(DayOfWeek.Monday) - CInt(today.DayOfWeek) + 7)
                        Dim nextMonday As DateTime = today.AddDays(daysUntilMonday)

                        Me.txtBeginDate.Date = nextMonday
                        Me.txtEndDate.Date = nextMonday.AddDays(6)
                    Case TypePeriodEnum.PeriodNextMonth
                        'Mes siguiente
                        Dim N = DateTime.Now.AddMonths(1)
                        Dim FirstMonthDay = New DateTime(N.Year, N.Month, 1)
                        Dim LastMonthDay = New DateTime(N.Year, N.Month, DateTime.DaysInMonth(N.Year, N.Month))

                        Me.txtBeginDate.Date = FirstMonthDay
                        Me.txtEndDate.Date = LastMonthDay
                    Case TypePeriodEnum.PeriodLastMonth
                        dtMonth = Now.AddMonths(-1)
                        Me.txtBeginDate.Date = New Date(dtMonth.Year, dtMonth.Month, 1)
                        Me.txtEndDate.Value = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
                    Case TypePeriodEnum.PeriodCurrentYear
                        'Año actual
                        Me.txtBeginDate.Date = New Date(Now.Year, 1, 1)
                        Me.txtEndDate.Value = Now
                    Case TypePeriodEnum.PeriodNMonthsAgoFromDay
                        'Mensual desde el día X de Y meses atrás
                        Dim dOneMonthAgo As Date = DateTime.Now.AddMonths(-1 * cmbMonthsShifted.Value)
                        Me.txtBeginDate.Date = DateSerial(dOneMonthAgo.Year, dOneMonthAgo.Month, cmbMonthsShiftedDay.Value)
                        Me.txtEndDate.Date = Me.txtBeginDate.Date.AddMonths(1).AddDays(-1)
                        iShiftedMonthMonthsInAdvance = cmbMonthsShifted.Value
                        iShiftedMonthStartDay = cmbMonthsShiftedDay.Value
                End Select
            Else
                eType = TypePeriodEnum.PeriodToday

                Me.txtBeginDate.Date = Now
                Me.txtEndDate.Date = Now
            End If

            If eType <> TypePeriodEnum.PeriodOther Then
                xBeginDate = New DateTime(Me.txtBeginDate.Date.Year, Me.txtBeginDate.Date.Month, Me.txtBeginDate.Date.Day, 0, 0, 0)
                xEndDate = New DateTime(Me.txtEndDate.Date.Year, Me.txtEndDate.Date.Month, Me.txtEndDate.Date.Day, 23, 59, 59)
            Else
                xBeginDate = New DateTime(Me.txtBeginDate.Date.Year, Me.txtBeginDate.Date.Month, Me.txtBeginDate.Date.Day, Me.txtBeginDate.Date.Hour, Me.txtBeginDate.Date.Minute, Me.txtBeginDate.Date.Second)
                xEndDate = New DateTime(Me.txtEndDate.Date.Year, Me.txtEndDate.Date.Month, Me.txtEndDate.Date.Day, Me.txtEndDate.Date.Hour, Me.txtEndDate.Date.Minute, Me.txtEndDate.Date.Second)
            End If
        Catch ex As Exception

        End Try

        Return bolret

    End Function

    Public Function SetValues(ByRef eType As TypePeriodEnum, ByRef xBeginDate As DateTime, ByRef xEndDate As DateTime, ByVal bolDisabled As Boolean, Optional iShiftedMonthStartDay As Integer = 1, Optional iShiftedMonthMonthsInAdvance As Integer = 1) As Boolean
        Dim bolret As Boolean = False
        Try
            If (rblPeriod.Items.Count() > 0) Then
                rblPeriod.Items.FindByValue(CInt(eType)).Selected = True
            End If

            Dim dtMonth As Date

            'Activación / Desactivación de los controles
            Me.txtBeginDate.ClientEnabled = (eType = TypePeriodEnum.PeriodOther)
            Me.txtEndDate.ClientEnabled = (eType = TypePeriodEnum.PeriodOther)

            If eType <> TypePeriodEnum.PeriodNMonthsAgoFromDay Then
                cmbMonthsShiftedDay.SelectedItem = cmbMonthsShiftedDay.Items.FindByValue(Now.AddDays(-1).Day.ToString)
                cmbMonthsShiftedDay.ClientEnabled = False
                cmbMonthsShifted.SelectedItem = cmbMonthsShifted.Items.FindByValue("1")
                cmbMonthsShifted.ClientEnabled = False
            Else
                cmbMonthsShiftedDay.ClientEnabled = True
                cmbMonthsShifted.ClientEnabled = True
            End If

            Select Case eType
                Case TypePeriodEnum.PeriodOther
                    Me.txtBeginDate.Date = xBeginDate
                    Me.txtEndDate.Date = xEndDate
                Case TypePeriodEnum.PeriodToday
                    Me.txtBeginDate.Date = Now
                    Me.txtEndDate.Date = Now
                Case TypePeriodEnum.PeriodTomorrow
                    Me.txtBeginDate.Date = Now.AddDays(1)
                    Me.txtEndDate.Date = Now.AddDays(1)
                Case TypePeriodEnum.PeriodYesterday
                    Me.txtBeginDate.Date = Now.AddDays(-1)
                    Me.txtEndDate.Value = Now.AddDays(-1)
                Case TypePeriodEnum.PeriodCurrentWeek
                    Me.txtBeginDate.Date = Now.AddDays(1 - Weekday(Now, vbMonday))
                    Me.txtEndDate.Value = Now.AddDays(7 - Weekday(Now, vbMonday))
                Case TypePeriodEnum.PeriodLastWeek
                    Me.txtBeginDate.Date = Now.AddDays(-6 - Weekday(Now, vbMonday))
                    Me.txtEndDate.Value = Now.AddDays(-Weekday(Now, vbMonday))
                Case TypePeriodEnum.PeriodNextWeek
                    'Semana siguiente
                    Dim today As DateTime = DateTime.Today
                    Dim daysUntilMonday As Integer = (CInt(DayOfWeek.Monday) - CInt(today.DayOfWeek) + 7)
                    Dim nextMonday As DateTime = today.AddDays(daysUntilMonday)

                    Me.txtBeginDate.Date = nextMonday
                    Me.txtEndDate.Date = nextMonday.AddDays(6)
                Case TypePeriodEnum.PeriodCurrentMonth
                    Me.txtBeginDate.Date = New Date(Now.Year, Now.Month, 1)
                    Me.txtEndDate.Value = New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day)
                Case TypePeriodEnum.PeriodLastMonth
                    dtMonth = Now.AddMonths(-1)
                    Me.txtBeginDate.Date = New Date(dtMonth.Year, dtMonth.Month, 1)
                    Me.txtEndDate.Value = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
                Case TypePeriodEnum.PeriodCurrentYear
                    'Año actual
                    Me.txtBeginDate.Date = New Date(Now.Year, 1, 1)
                    Me.txtEndDate.Value = Now
                Case TypePeriodEnum.PeriodNextMonth
                    'Mes siguiente
                    Dim N = DateTime.Now.AddMonths(1)
                    Dim FirstMonthDay = New DateTime(N.Year, N.Month, 1)
                    Dim LastMonthDay = New DateTime(N.Year, N.Month, DateTime.DaysInMonth(N.Year, N.Month))

                    Me.txtBeginDate.Date = FirstMonthDay
                    Me.txtEndDate.Date = LastMonthDay
                Case TypePeriodEnum.PeriodNMonthsAgoFromDay
                    'Mensual desde el día X del mes anterior
                    cmbMonthsShiftedDay.SelectedItem = cmbMonthsShiftedDay.Items.FindByValue(iShiftedMonthStartDay.ToString)
                    cmbMonthsShifted.SelectedItem = cmbMonthsShifted.Items.FindByValue(iShiftedMonthMonthsInAdvance.ToString)
                    Me.txtBeginDate.Date = xBeginDate
                    Me.txtEndDate.Date = xEndDate
            End Select

            If eType <> TypePeriodEnum.PeriodOther Then
                Me.txtBeginDate.Date = New DateTime(Me.txtBeginDate.Date.Year, Me.txtBeginDate.Date.Month, Me.txtBeginDate.Date.Day, 0, 0, 0)
                Me.txtEndDate.Date = New DateTime(Me.txtEndDate.Date.Year, Me.txtEndDate.Date.Month, Me.txtEndDate.Date.Day, 23, 59, 59)
            Else
                Me.txtBeginDate.Date = New DateTime(Me.txtBeginDate.Date.Year, Me.txtBeginDate.Date.Month, Me.txtBeginDate.Date.Day, Me.txtBeginDate.Date.Hour, Me.txtBeginDate.Date.Minute, Me.txtBeginDate.Date.Second)
                Me.txtEndDate.Date = New DateTime(Me.txtEndDate.Date.Year, Me.txtEndDate.Date.Month, Me.txtEndDate.Date.Day, Me.txtEndDate.Date.Hour, Me.txtEndDate.Date.Minute, Me.txtEndDate.Date.Second)
            End If

            If bolDisabled Then
                rblPeriod.ClientEnabled = Not bolDisabled
                txtBeginDate.ClientEnabled = Not bolDisabled
                txtEndDate.ClientEnabled = Not bolDisabled
            End If
        Catch ex As Exception

        End Try

        Return True
    End Function

    Private Sub roOptSchedulePeriodPanel_Callback(sender As Object, e As CallbackEventArgsBase) Handles roOptSchedulePeriodPanel.Callback

        Dim eType As TypePeriodEnum = TypePeriodEnum.PeriodCurrentMonth
        Dim iDate As DateTime = DateTime.Now
        Dim edate As DateTime = DateTime.Now
        Dim iShiftedMonthStartDay As Integer = 1
        Dim iShiftedMonthMonthsInAdvance As Integer = 1
        Me.GetValues(eType, iDate, edate, iShiftedMonthStartDay, iShiftedMonthMonthsInAdvance)

        If roOptSchedulePeriodPanel.JSProperties.ContainsKey("cp_beginDate") Then
            roOptSchedulePeriodPanel.JSProperties("cp_beginDate") = iDate
        Else
            roOptSchedulePeriodPanel.JSProperties.Add("cp_beginDate", iDate)
        End If
        If roOptSchedulePeriodPanel.JSProperties.ContainsKey("cp_endDate") Then
            roOptSchedulePeriodPanel.JSProperties("cp_endDate") = edate
        Else
            roOptSchedulePeriodPanel.JSProperties.Add("cp_endDate", edate)
        End If

    End Sub

End Class