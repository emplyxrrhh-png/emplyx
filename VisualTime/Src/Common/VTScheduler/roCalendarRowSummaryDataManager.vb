Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTCalendar

    Public Class roCalendarRowSummaryDataManager
        Private oState As roCalendarRowSummaryDataState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowSummaryDataState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowSummaryDataState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xDate As DateTime, ByVal xLastDate As DateTime, ByVal intIDEmployee As Integer, ByVal oParams As roParameters, ByVal oConcept As Concept.roConcept, ByVal idShiftHolidays As Integer, ByVal oConceptHolidays As Concept.roConcept, ByRef oStateEmployee As Employee.roEmployeeState, ByVal oCalRulesManager As VTCalendar.roCalendarScheduleRulesManager, ByRef oProgrammedAbsState As Absence.roProgrammedAbsenceState, ByRef oProgrammedHolidaysState As roProgrammedHolidayState, ByVal oProgrammedHolidayManager As roProgrammedHolidayManager, ByVal oProgrammedOvertimeyManager As roProgrammedOvertimeManager, ByRef oProgrammedOvertimesState As roProgrammedOvertimeState, ByVal tbProgrammedAbsences As DataTable) As roCalendarRowSummaryData

            Dim oRet As New roCalendarRowSummaryData
            Dim bolRet As Boolean = False

            Try

                Dim lstDates As Generic.List(Of DateTime) = Nothing

                oRet.Accrual = Concept.roConcept.GetAccrualValueOnDate(intIDEmployee, oParams, xDate.AddDays(-1), False, oConcept, oStateEmployee, lstDates)

                If xDate.Day = 1 AndAlso xDate.Month = 1 Then
                    oRet.AccrualHolidays = Format(Concept.roConcept.GetAccrualValueOnDate(intIDEmployee, oParams, xDate, False, oConceptHolidays, oStateEmployee, lstDates), "##0.000")
                Else
                    oRet.AccrualHolidays = Format(Concept.roConcept.GetAccrualValueOnDate(intIDEmployee, oParams, xDate.AddDays(-1), False, oConceptHolidays, oStateEmployee, lstDates), "##0.000")
                End If

                If idShiftHolidays > 0 Then
                    Dim intDone As Double = 0
                    Dim intPending As Double = 0
                    Dim intLasting As Double = 0
                    Dim intDisponible As Double = 0
                    Dim intExpired As Double = 0
                    Dim WithoutEnjoyment As Double = 0


                    Common.roBusinessSupport.VacationsResumeQuery(intIDEmployee, idShiftHolidays, Now.Date, Nothing, Nothing, Now.Date, intDone, intPending, intLasting, intDisponible, oState, intExpired, WithoutEnjoyment)

                    oRet.HolidayResume.Done = intDone
                    oRet.HolidayResume.Pending = intPending
                    oRet.HolidayResume.Requested = intLasting
                End If

                Dim iMonthIniDay As Integer
                Dim iYearIniMonth As Integer
                Dim dYearFirstDate As Date = oCalRulesManager.GetYearFirstDate(xDate, oParams, iMonthIniDay, iYearIniMonth)
                Dim dYearEndDate As Date = dYearFirstDate.AddYears(1).AddDays(-1)

                oRet.PlannedHours.YearTotal = Me.GetEmployeePlannedHoursInPeriod(intIDEmployee, xDate, xLastDate, dYearFirstDate)
                oRet.PlannedHours.AccruedToDate = Me.GetEmployeePlannedHoursInPeriod(intIDEmployee, xDate, dYearEndDate, dYearFirstDate)

                ' Obtenemos las alertas a dia de hoy
                oRet.Alerts = New roCalendarRowDayAlerts

                ' Obtenemos la planificacion del dia de hoy
                Dim tbDetail As DataTable = VTBusiness.Scheduler.roScheduler.GetPlan(intIDEmployee, Now.Date, Now.Date, oStateEmployee, , True)
                Dim oRows As DataRow()

                ' Obtenemos las previsiones de ausencia dias/horas
                Dim tbProgrammedCauses As DataTable = Absence.roProgrammedAbsence.GetProgrammedCauses(intIDEmployee, Now.Date, Now.Date, oProgrammedAbsState)

                ' Obtenemos las previsiones de vacaciones por horas
                Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(intIDEmployee, oProgrammedHolidaysState, "Date=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime)

                ' Obtenemos las previsiones de horas de exceso
                Dim lstProgrammedOvertimes As New Generic.List(Of roProgrammedOvertime)
                lstProgrammedOvertimes = oProgrammedOvertimeyManager.GetProgrammedOvertimes(intIDEmployee, oProgrammedOvertimesState, "BeginDate <=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " AND EndDate >=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime)

                oRet.Alerts.OnAbsenceDays = False
                oRows = tbProgrammedAbsences.Select("(BeginDate <= '" & Format(Now.Date, "yyyy/MM/dd") & "' AND " &
                                                            "RealFinishDate >= '" & Format(Now.Date, "yyyy/MM/dd") & "')")
                If oRows.Length > 0 Then oRet.Alerts.OnAbsenceDays = True

                oRet.Alerts.OnAbsenceHours = False
                oRows = tbProgrammedCauses.Select("Date <= '" & Format(Now.Date, "yyyy/MM/dd") & "' and isnull(FinishDate, date) >= '" & Format(Now.Date, "yyyy/MM/dd") & "'")
                If oRows.Length > 0 Then oRet.Alerts.OnAbsenceHours = True

                oRet.Alerts.OnHolidaysHours = False
                If lstProgrammedHolidays.Count > 0 Then oRet.Alerts.OnHolidaysHours = True

                oRet.Alerts.OnOvertimesHours = False
                If lstProgrammedOvertimes.Count > 0 Then oRet.Alerts.OnOvertimesHours = True

                oRet.Alerts.OnHolidays = False
                oRet.Alerts.UnexpectedlyAbsent = False

                oRows = tbDetail.Select("Date = '" & Format(Now.Date, "yyyy/MM/dd") & "'")
                If oRows.Length > 0 Then
                    oRet.Alerts.OnHolidays = roTypes.Any2Boolean(oRows(0)("IsHolidays"))
                    Dim oEmployeeStatus As Employee.roEmployeeStatus
                    oEmployeeStatus = New Employee.roEmployeeStatus(intIDEmployee, New Employee.roEmployeeState(oState.IDPassport))
                    If roTypes.Any2Integer(oRows(0)("IDShiftUsed")) > 0 Then
                        If Not oEmployeeStatus.IsPresent AndAlso (Not oEmployeeStatus.BeginMandatory.HasValue OrElse oEmployeeStatus.BeginMandatory.Value < Now) Then
                            oRet.Alerts.UnexpectedlyAbsent = True
                        End If
                    End If
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowSummaryDataManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowSummaryDataManager::Load")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetEmployeePlannedHoursInPeriod(idEmployee As Integer, dStartDate As Date, dEndDate As Date, dBeginYearDate As Date) As Double
            Dim oRet As Double

            Try

                Dim strSQL As String = String.Empty

                strSQL = "@SELECT# SUM(isnull(ds.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)) " &
                "From DailySchedule ds " &
                "inner join Shifts on Shifts.ID = ds.IDShift1 " &
                "inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate " &
                "where ds.IDEmployee = " & idEmployee & " " &
                "and Date between " & roTypes.Any2Time(dBeginYearDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dBeginYearDate.AddYears(1).AddDays(-1)).SQLSmallDateTime & " " &
                "And Date not between " & roTypes.Any2Time(dStartDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dEndDate).SQLSmallDateTime & " " &
                "group by ds.IDEmployee, ec.IDContract, ec.IDLabAgree"

                oRet = roTypes.Any2Double(ExecuteScalar(strSQL))
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowSummaryDataManager::GetEmployeePlannedHoursInPeriod")
                Return Nothing
            Finally

            End Try
            Return oRet
        End Function

#End Region

    End Class

End Namespace