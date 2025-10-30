Imports Robotics.Web.Base

Partial Class WebUserForms_frmAdvCopy
    Inherits UserControlBase

    Public Property WorkMode As roCalendar.roCalendarWorkMode
        Get
            If ViewState("roAdvCopyWorkMode") Is Nothing Then
                Return False
            Else
                Return ViewState("roAdvCopyWorkMode")
            End If
        End Get
        Set(value As roCalendar.roCalendarWorkMode)
            ViewState("roAdvCopyWorkMode") = value
        End Set
    End Property

    Private Sub WebUserForms_frmAdvCopy_Init(sender As Object, e As EventArgs) Handles Me.Init
    End Sub

    Private Sub WebUserForms_frmAdvCopy_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Dim bolTelecommuting As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")

            txtToDate.Date = Date.Now.Date
            txtRepeatTimes.Value = 1

            cmbRepeatStartsDay.Items.Clear()
            cmbSkipsWeekDay.Items.Clear()

            For Each oValue As DayOfWeek In System.Enum.GetValues(GetType(DayOfWeek))

                If oValue <> DayOfWeek.Sunday Then
                    cmbRepeatStartsDay.Items.Add(Me.Language.Keyword("weekday." & CInt(oValue)), CInt(oValue))
                    cmbSkipsWeekDay.Items.Add(Me.Language.Keyword("weekday." & CInt(oValue)), CInt(oValue))
                End If

            Next
            cmbRepeatStartsDay.Items.Add(Me.Language.Keyword("weekday.7"), CInt(DayOfWeek.Sunday))
            cmbSkipsWeekDay.Items.Add(Me.Language.Keyword("weekday.7"), CInt(DayOfWeek.Sunday))

            cmbSkipsWeekDay.SelectedItem = cmbSkipsWeekDay.Items.FindByValue("1")
            cmbRepeatStartsDay.SelectedItem = cmbRepeatStartsDay.Items.FindByValue("1")

            rbRepeatUntil.ClientInstanceName = Me.ClientID & "_rbRepeatUntilClient"
            txtToDate.ClientInstanceName = Me.ClientID & "_txtToDateClient"

            rbRepeatTimes.ClientInstanceName = Me.ClientID & "_rbRepeatTimesClient"
            txtRepeatTimes.ClientInstanceName = Me.ClientID & "_txtRepeatTimesClient"

            ckBloqDestDays.ClientInstanceName = Me.ClientID & "_ckBloqDestDaysClient"

            rbAdvancedRepeatDisabled.ClientInstanceName = Me.ClientID & "_rbAdvancedRepeatDisabledClient"
            rbAdvancedRepeatEnabled.ClientInstanceName = Me.ClientID & "_rbAdvancedRepeatEnabledClient"

            rbAdvancedBloquedDisabled.ClientInstanceName = Me.ClientID & "_rbAdvancedBloquedDisabledClient"
            rbAdvancedBloquedEnabled.ClientInstanceName = Me.ClientID & "_rbAdvancedBloquedEnabledClient"

            rbAdvancedHolidaysDisabled.ClientInstanceName = Me.ClientID & "_rbAdvancedHolidaysDisabledClient"
            rbAdvancedHolidaysEnabled.ClientInstanceName = Me.ClientID & "_rbAdvancedHolidaysEnabledClient"

            rbStartsInmediately.ClientInstanceName = Me.ClientID & "_rbStartsInmediately"
            rbStartsNextDay.ClientInstanceName = Me.ClientID & "_rbStartsNextDayClient"
            cmbRepeatStartsDay.ClientInstanceName = Me.ClientID & "_cmbRepeatStartsDayClient"
            rbStartsNextMonth.ClientInstanceName = Me.ClientID & "_rbStartsNextMonthClient"
            txtRepeatStartsMonth.ClientInstanceName = Me.ClientID & "_txtRepeatStartsMonthClient"

            ckSkipOptions.ClientInstanceName = Me.ClientID & "_ckSkipOptionsClient"
            txtSkipOptions.ClientInstanceName = Me.ClientID & "_txtSkipOptionsClient"
            rbRepeatSkipWeek.ClientInstanceName = Me.ClientID & "_rbRepeatSkipWeekClient"
            cmbSkipsWeekDay.ClientInstanceName = Me.ClientID & "_cmbSkipsWeekDayClient"
            rbRepeatSkipMonth.ClientInstanceName = Me.ClientID & "_rbRepeatSkipMonthClient"
            txtSkipMonthDayValue.ClientInstanceName = Me.ClientID & "_txtSkipMonthDayValueClient"
            rbRepeatSkipDays.ClientInstanceName = Me.ClientID & "_rbRepeatSkipDaysClient"
            txtRepeatSkipDays.ClientInstanceName = Me.ClientID & "_txtRepeatSkipDaysClient"

            rbHolidaySkip.ClientInstanceName = Me.ClientID & "_rbHolidaySkipClient"
            rbHolidayIgnore.ClientInstanceName = Me.ClientID & "_rbHolidayIgnoreClient"
            rbHolidayOverwrite.ClientInstanceName = Me.ClientID & "_rbHolidayOverwriteClient"

            rbBloquedSkip.ClientInstanceName = Me.ClientID & "_rbBloquedSkipClient"
            rbBloquedIgnore.ClientInstanceName = Me.ClientID & "_rbBloquedIgnoreClient"
            rbBloquedOverWrite.ClientInstanceName = Me.ClientID & "_rbBloquedOverWriteClient"

            rbTelecommuteKeep.ClientInstanceName = Me.ClientID & "_rbTelecommuteKeepClient"
            rbTelecommuteCopy.ClientInstanceName = Me.ClientID & "_rbTelecommuteCopyClient"
            rbTelecommuteDefault.ClientInstanceName = Me.ClientID & "_rbTelecommuteDefaultClient"

            If bolTelecommuting Then
                Me.divTelecommuteOptions.Style("display") = ""
            Else
                Me.divTelecommuteOptions.Style("display") = "none"
            End If

            Label8.Attributes.Add("onclick", Me.ClientID & "_showHolidaysPopup()")
            Label4.Attributes.Add("onclick", Me.ClientID & "_showBloquedPopup()")
            Label1.Attributes.Add("onclick", Me.ClientID & "_showRepeatPopup()")
        End If

        Me.lblEspecialPasteInfo.Text = Me.Language.Translate("lblEspecialPasteInfo." & Me.WorkMode.ToString(), Me.DefaultScope)
        Me.lblAdvancedHolidaysDisabled.Text = Me.Language.Translate("lblAdvancedHolidaysDisabled." & Me.WorkMode.ToString(), Me.DefaultScope)
        Me.lblShiftsDesc.Text = Me.Language.Translate("lblShiftsDesc." & Me.WorkMode.ToString(), Me.DefaultScope)
        Me.lblHolidaySkip.Text = Me.Language.Translate("lblHolidaySkip." & Me.WorkMode.ToString(), Me.DefaultScope)
        Me.lblHolidaySkipDesc.Text = Me.Language.Translate("lblHolidaySkipDesc." & Me.WorkMode.ToString(), Me.DefaultScope)

    End Sub

    Public Sub InitLanguages(ByVal oMode As VTLive40.roCalendar.roCalendarWorkMode)
        Me.WorkMode = oMode
    End Sub

End Class