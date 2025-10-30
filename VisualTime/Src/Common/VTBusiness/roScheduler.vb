Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotifications
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Scheduler

    <DataContract()>
    Public Class roScheduler

        'Public Enum AnalyticsTypeEnum
        '    <EnumMember> _NOTDEFINDED = 0 ' No definido
        '    <EnumMember> _SCHEDULER = 1     ' Entrada
        '    <EnumMember> _ACCESS = 2    ' Salida
        '    <EnumMember> _PRODUCTIV = 3   ' Presencia Automatica
        '    <EnumMember> _COSTCENTERS = 4   ' Presencia Automatica
        'End Enum

        ''' <summary>
        ''' Devuelve un dataset con los datos de los empleados de la lista pasada por parámetro
        ''' La lista tiene que ser los codigos separados por comas: 1,4,5,14,23
        ''' Si la lista pasada no contiene ninguna coma se considera que se quiere hacer un where por un valor
        ''' </summary>
        ''' <param name="List"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetScheduledEmployeesFromList(ByVal List As String,
                                             ByRef oState As roSchedulerState) As DataTable
            Dim strQuery As String
            Dim oDataSet As DataSet = Nothing

            Try

                strQuery = "@SELECT# ID IDEmployee, '-1' IDGroup, Name, Path From Employees "
                strQuery &= " INNER Join sysrovwCurrentEmployeeGroups vceg with (nolock) on vceg.IDEmployee = Employees.ID "
                strQuery &= " INNER Join sysrovwSecurity_PermissionOverEmployees poe With (NOLOCK) On poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = Employees.ID And CONVERT(date,GETDATE()) between poe.BeginDate And poe.EndDate "


                If List <> "" Then
                    strQuery = strQuery & " Where "
                    If InStr(List, ",") > 0 Then
                        strQuery = strQuery & " ID IN (" & List & ")"
                    Else
                        strQuery = strQuery & " ID  = " & List
                    End If
                Else
                    'strQuery = strQuery & " Where "
                    'strQuery &= " ID = 0"
                End If
                strQuery &= " ORDER BY Employees.Name"

                oDataSet = CreateDataSet(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roScheduler::GetEmployeeFromList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roScheduler::GetEmployeeFromList")
            End Try

            Return oDataSet.Tables(0)

        End Function

#Region "Scheduler Employee Helper methods"

        Public Shared Function GetPlan(ByVal intIDEmployee As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByRef oState As Employee.roEmployeeState,
                                 Optional ByVal IdGroup As Integer = -1, Optional ByVal excludePermission As Boolean = False) As DataTable
            '
            ' Obtiene la planificación del empleado en el periodo indicado
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try
                ' Miramos si el passport actual es de tipo empleado o no, para establecer los permisos correspondientes ('U' o 'E').
                Dim bolPassportEmployee As Boolean = (oState.ActivePassportType() = "E")

                ' Verificamos los permisos del pasaporte actual sobre la planificación
                Dim bolPermission As Boolean = True
                If bolPassportEmployee Then
                    bolPermission = WLHelper.HasFeaturePermission(oState.IDPassport, "Planification.Query", Permission.Read, "E")
                End If

                If bolPermission Or excludePermission Then

                    Dim strSQL As String
                    Dim iAssignCount As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar("@SELECT# count(ID) from Assignments"))

                    strSQL = " @SELECT# " &
                                        "Shifts.Name AS Name1, " &
                                        "NULL AS Name2, " &
                                        "NULL AS Name3, " &
                                        "NULL AS Name4, " &
                                        "NULL AS NameBase, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.Name END AS UsedName, " &
                                        "DailySchedule.Date, " &
                                        "DailySchedule.Telecommuting, " &
                                        "DailySchedule.TelecommutingOptional, " &
                                        "DailySchedule.WorkCenter, " &
                                        "Shifts.ShortName AS ShortName1, NULL AS ShortName2, NULL AS ShortName3, NULL AS ShortName4, ShiftsBase.ShortName AS ShortNameBase, " &
                                        "ISNULL(DailySchedule.ShiftColor1,Shifts.Color) AS PrimaryColor, " &
                                        "ISNULL(DailySchedule.ShiftColor1,Shifts.Color) AS ShiftColor1, " &
                                        "NULL AS ShiftColor2, " &
                                        "NULL AS ShiftColor3, " &
                                        "NULL AS ShiftColor4, " &
                                        "ISNULL(DailySchedule.ShiftColorUsed,CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.Color END) AS UsedColor, " &
                                        "DailySchedule.Status, " &
                                        "DailySchedule.IDShift1, DailySchedule.IDShift2, DailySchedule.IDShift3, DailySchedule.IDShift4, DailySchedule.IDPreviousShift," &
                                        "DailySchedule.IDEmployee, DailySchedule.LockedDay, DailySchedule.FeastDay,DailySchedule.IDDailyBudgetPosition, " &
                                        "(@SELECT# Name from ProductiveUnits WHERE ID = (@SELECT# IDProductiveUnit FROM DailyBudgets where ID IN(@SELECT# IDDailyBudget from DailyBudget_Positions where id=DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName, " &
                                        "(@SELECT# top 1 isnull(Description,'') from sysroScheduleTemplates_Detail WHERE ScheduleDate = DailySchedule.Date Order by IDTemplate desc) as FeastDayDescription, " &
                                        "DailySchedule.IDShiftUsed, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.ShortName END AS UsedShortName, " &
                                        "Shifts.Description as Description1, " &
                                        "DailySchedule.StartShiftUsed, " &
                                        "DailySchedule.StartShift1, " &
                                        "DailySchedule.StartShift2, " &
                                        "DailySchedule.StartShift3, " &
                                        "DailySchedule.StartShift4, " &
                                        "DailySchedule.StartShiftBase, " &
                                        "DailySchedule.StartFlexibleUsed, " &
                                        "DailySchedule.StartFlexible1, " &
                                        "DailySchedule.StartFlexibleBase, " &
                                        "DailySchedule.EndFlexibleUsed, " &
                                        "DailySchedule.EndFlexible1, " &
                                        "DailySchedule.EndFlexibleBase, " &
                                        "DailySchedule.IDAssignment, DailySchedule.IsCovered, DailySchedule.OldIDAssignment, DailySchedule.IDEmployeeCovered, "

                    If (iAssignCount > 0) Then
                        strSQL = strSQL & "Assignments.Color AS AssignmentColor, Assignments.Name AS AssignmentName, Assignments.ShortName AS AssignmentShortName, " &
                                        "OldAssignments.Color AS OldAssignmentColor, OldAssignments.Name AS OldAssignmentName, OldAssignments.ShortName AS OldAssignmentShortName, "
                    Else
                        strSQL = strSQL & "NULL AS AssignmentColor, NULL AS AssignmentName, NULL AS AssignmentShortName, " &
                                        "NULL AS OldAssignmentColor, NULL AS OldAssignmentName, NULL AS OldAssignmentShortName, "
                    End If

                    strSQL = strSQL & "DailySchedule.IDAssignmentBase, DailySchedule.IsHolidays, DailySchedule.IDShiftBase, " &
                                        "CASE ISNULL(IsCovered,0) WHEN 0 " &
                                            "THEN DailySchedule.IDEmployeeCovered " &
                                            "ELSE (@SELECT# DS.IDEmployee FROM DailySchedule DS with (nolock) WHERE DS.Date = DailySchedule.Date AND DS.IDEmployeeCovered = DailySchedule.IDEmployee ) END  AS 'CoverageIDEmployee', " &
                                        "CASE ISNULL(IsCovered,0) WHEN 0 " &
                                            "THEN (@SELECT# Employees.Name FROM Employees with (nolock) WHERE Employees.ID = DailySchedule.IDEmployeeCovered) " &
                                            "ELSE (@SELECT# Employees.Name FROM DailySchedule DS with (nolock) INNER JOIN Employees ON DS.IDEmployee = Employees.ID WHERE DS.Date = DailySchedule.Date AND DS.IDEmployeeCovered = DailySchedule.IDEmployee ) END  AS 'CoverageEmployeeName'," &
                                        "isnull(DailySchedule.ExpectedWorkingHours,CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.ExpectedWorkingHours END) As ExpectedWorkingHoursUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.BreakHours END As BreakHoursUsedShift," &
                                        "ISNULL(DailySchedule.ShiftNameUsed,CASE ISNULL(IDShiftUsed,0) WHEN 0 THEN '' ELSE Shifts.Name END) As NameUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.ShiftType END As ShiftTypeUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.StartLimit END as StartLimitUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.EndLimit END as EndLimitUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.StartFloating END as StartFloatingUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.IsFloating END As IsFloatingUsedShift," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.Export END As ExportUsed," &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.WhoToNotifyBefore END As WhoToNotifyBefore, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.WhoToNotifyAfter END As WhoToNotifyAfter, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.NotifyEmployeeBeforeAt END As NotifyEmployeeBeforeAt, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.NotifyEmployeeAfterAt END As NotifyEmployeeAfterAt, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.EnableNotifyBefore END As EnableNotifyBefore, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.EnableNotifyAfter END As EnableNotifyAfter, " &
                                        "isnull(DailySchedule.ExpectedWorkingHours,Shifts.ExpectedWorkingHours) As ExpectedWorkingHours1," &
                                        "Shifts.BreakHours As BreakHours1," &
                                        "ISNULL(DailySchedule.ShiftName1,Shifts.Name) As NameShift1," &
                                        "Shifts.ShiftType  As ShiftType1," &
                                        "Shifts.StartLimit as StartLimit1," &
                                        "Shifts.EndLimit as EndLimit1," &
                                        "Shifts.StartFloating as StartFloating1," &
                                        "Shifts.IsFloating  As IsFloating1," &
                                        "Shifts.Export  As Export1," &
                                        "NULL AS ExpectedWorkingHours2," &
                                        "NULL As NameShift2," &
                                        "NULL AS ShiftType2," &
                                        "NULL As StartLimit2," &
                                        "NULL As EndLimit2," &
                                        "NULL As StartFloating2," &
                                        "NULL as IsFloating2," &
                                        "NULL AS ExpectedWorkingHours3," &
                                        "NULL As NameShift3," &
                                        "NULL AS ShiftType3," &
                                        "NULL As StartLimit3," &
                                        "NULL As EndLimit3," &
                                        "NULL As StartFloating3," &
                                        "NULL as IsFloating3," &
                                        "NULL AS ExpectedWorkingHours4," &
                                        "NULL As NameShift4," &
                                        "NULL AS ShiftType4," &
                                        "NULL As StartLimit4," &
                                        "NULL As EndLimit4," &
                                        "NULL As StartFloating4," &
                                        "NULL as IsFloating4," &
                                        "isnull(DailySchedule.ExpectedWorkingHours,ShiftsBase.ExpectedWorkingHours) AS ExpectedWorkingHoursBase," &
                                        "ShiftsBase.BreakHours As BreakHoursBase," &
                                        "ISNULL(DailySchedule.ShiftNameBase,ShiftsBase.Name) As NameShiftBase," &
                                        "ShiftsBase.ShiftType  AS ShiftTypeBase," &
                                        "ShiftsBase.StartLimit As StartLimitBase," &
                                        "ShiftsBase.EndLimit As EndLimitBase," &
                                        "ShiftsBase.StartFloating As StartFloatingBase," &
                                        "ShiftsBase.IsFloating  as IsFloatingBase," &
                                        "ISNULL(DailySchedule.ShiftColorBase,ShiftsBase.Color)  as ShiftColorBase," &
                                        "ISNULL(CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.AreWorkingDays END,0) As AreWorkingDaysUsedShift," &
                                        "ISNULL(Shifts.AreWorkingDays,0) As AreWorkingDays1," &
                                        "0 As AreWorkingDays2," &
                                        "0 As AreWorkingDays3," &
                                        "0 As AreWorkingDays4," &
                                        "0 As AreWorkingDaysBase," &
                                        "DailySchedule.Remarks, " &
                                        "Shifts.AllowFloatingData as ExistFloatingData, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.AllowFloatingData END as ExistFloatingDataUsedShift, " &
                                        "ShiftsBase.AllowFloatingData as ExistFloatingDataBase, " &
                                        "CASE IDShiftUsed WHEN NULL THEN NULL ELSE Shifts.AllowComplementary END as ExistComplementaryDataUsedShift, " &
                                        "Shifts.AllowComplementary as ExistComplementaryDataShift1, " &
                                        "ShiftsBase.AllowComplementary as ExistComplementaryDataBase, " &
                                        "isnull(LayersDefinition,'') as LayersDefinition, " &
                                        "isnull(DailySchedule.IDAssignmentBase,DailySchedule.IDAssignment) as IDAssignmentFix, "

                    If (iAssignCount > 0) Then
                        strSQL = strSQL & " (@SELECT# isnull(Coverage,0) from ShiftAssignments where ShiftAssignments.IDShift = isnull(DailySchedule.IDShiftBase,DailySchedule.IDShift1) and ShiftAssignments.IDAssignment  = isnull(DailySchedule.IDAssignmentBase,DailySchedule.IDAssignment) ) as CoverageAssignment ," &
                                        " (@SELECT# count(*) from ShiftAssignments where ShiftAssignments.IDShift = isnull(DailySchedule.IDShiftBase,DailySchedule.IDShift1) ) as AvailableAssignments ," &
                                        "AssignmentsFix.Color as AssignmentFixColor, " &
                                        "AssignmentsFix.Name as AssignmentFixName, " &
                                        "AssignmentsFix.ShortName as AssignmentFixShortName, "
                    Else
                        strSQL = strSQL & "NULL as CoverageAssignment ," &
                                        "NULL as AvailableAssignments ," &
                                        "NULL as AssignmentFixColor, " &
                                        "NULL as AssignmentFixName, " &
                                        "NULL as AssignmentFixShortName, "
                    End If

                    strSQL = strSQL & "CASE ISNULL(IDShiftUsed,0) WHEN 0 THEN '' ELSE isnull(Shifts.AdvancedParameters,'') END As AdvancedParametersUsed," &
                                        "isnull(ShiftsBase.AdvancedParameters,'') As AdvancedParametersBase," &
                                        "isnull(Shifts.AdvancedParameters,'') As AdvancedParameters1," &
                                        "'' As AdvancedParameters2," &
                                        "'' As AdvancedParameters3," &
                                        "'' As AdvancedParameters4, " &
                                        "tc.Telecommuting AS TelecommutingFromView, " &
                                        "tc.TelecommutingMandatoryDays, " &
                                        "tc.PresenceMandatoryDays, " &
                                        "tc.TelecommutingOptionalDays, " &
                                        "tc.TelecommutingMaxDays, " &
                                        "tc.TelecommutingMaxPercentage, " &
                                        "tc.PeriodType as TelecommutingPeriodType, " &
                                        "DailySchedule.Timestamp as Timestamp " &
                                   "FROM DailySchedule With (nolock) " &
                                         "LEFT JOIN sysrovwTelecommutingAgreement tc with (nolock) on tc.IDEmployee = dbo.DailySchedule.IDEmployee  and dbo.DailySchedule.Date BETWEEN tc.ContractStart AND tc.ContractEnd  and dbo.DailySchedule.Date BETWEEN tc.TelecommutingAgreementStart AND tc.TelecommutingAgreementEnd " &
                                         "LEFT OUTER JOIN Shifts With (nolock) On dbo.DailySchedule.IDShift1 = dbo.Shifts.ID " &
                                         "LEFT OUTER JOIN Shifts ShiftsBase With (nolock) On dbo.DailySchedule.IDShiftBase = ShiftsBase.ID "

                    If (iAssignCount > 0) Then
                        strSQL = strSQL & "LEFT OUTER JOIN Assignments With (nolock) On dbo.DailySchedule.IDAssignment = Assignments.ID " &
                                         "LEFT OUTER JOIN Assignments OldAssignments With (nolock) On dbo.DailySchedule.OldIDAssignment = OldAssignments.ID " &
                                         "LEFT OUTER JOIN Assignments AssignmentsFix With (nolock) On isnull(DailySchedule.IDAssignmentBase,DailySchedule.IDAssignment) = AssignmentsFix.ID " &
                                         "LEFT OUTER JOIN Employees With (nolock) On dbo.DailySchedule.IDEmployeeCovered = Employees.ID "
                    End If

                    If IdGroup <> -1 Then
                        strSQL &= "INNER JOIN EmployeeGroups With (nolock) On DailySchedule.IDEmployee = EmployeeGroups.IDEmployee And DailySchedule.Date >= EmployeeGroups.BeginDate And DailySchedule.Date <= EmployeeGroups.EndDate "
                    End If

                    If Not excludePermission Then
                        If Not bolPassportEmployee Then
                            strSQL &= " INNER Join sysrovwSecurity_PermissionOverEmployeeAndFeature poe with (NOLOCK) on poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = DailySchedule.IDEmployee And DateAdd(Day, DateDiff(Day, 0, DailySchedule.Date), 0) between poe.BeginDate And poe.EndDate and poe.idFeature=2 and poe.FeaturePermission > 0 "
                        End If
                    End If

                    strSQL &= "WHERE DailySchedule.IDEmployee = " & intIDEmployee & " And " &
                                  "DailySchedule.Date >= " & Any2Time(xBegin.Date).SQLSmallDateTime & " And DailySchedule.Date <= " & Any2Time(xEnd.Date).SQLSmallDateTime


                    If IdGroup <> -1 Then
                        strSQL &= " And EmployeeGroups.IdGroup = " & IdGroup
                    End If

                    tbPlan = CreateDataTableWithoutTimeouts(strSQL,, "Plan")
                Else
                    oState.Result = EmployeeResultEnum.AccessDenied
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetPlan")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Shared Function GetScheduleStatus(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByRef oState As Employee.roEmployeeState) As DataTable
            '
            ' Obtiene estado de calculo de presencia
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String
                strSQL = " @SELECT# DailySchedule.Status, DailySchedule.IDEmployee " &
                         " FROM dbo.DailySchedule " &
                         " WHERE DailySchedule.IDEmployee = " & intIDEmployee.ToString & " AND " &
                               "DailySchedule.Date = " & Any2Time(xDate.Date).SQLSmallDateTime
                tbPlan = CreateDataTableWithoutTimeouts(strSQL, , "SchedulerPlan")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetScheduleStatus")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetScheduleStatus")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Shared Function GetTaskPlan(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByRef oState As Employee.roEmployeeState) As DataTable
            '
            ' Obtiene estado de calculo de tareas
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String
                strSQL = " @SELECT# DailySchedule.TaskStatus as Status, DailySchedule.IDEmployee " &
                         " FROM dbo.DailySchedule " &
                         " WHERE DailySchedule.IDEmployee = " & intIDEmployee.ToString & " AND " &
                               "DailySchedule.Date = " & Any2Time(xDate.Date).SQLSmallDateTime
                tbPlan = CreateDataTableWithoutTimeouts(strSQL,, "TaskPlan")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetTaskPlan")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Shared Function ParseIDShifts(ByVal strIDShifts As String) As Integer()

            Dim Ret() As Integer = {0, 0, 0, 0, 0}

            Dim strID As String = ""
            For n As Integer = 0 To 4
                If strIDShifts.Split("*").Length > n Then
                    strID = strIDShifts.Split("*")(n).Split("~")(0)
                    If IsNumeric(strID) Then
                        Ret(n) = Val(strID)
                    End If
                End If
            Next

            Return Ret

        End Function

        Public Shared Function ParseStartShifts(ByVal strShifts As String) As DateTime()

            Dim Ret() As DateTime = {Nothing, Nothing, Nothing, Nothing, Nothing}

            Dim strShiftInfo As String = ""
            Dim strStart As String = ""
            For n As Integer = 0 To 4
                If strShifts.Split("*").Length > n Then
                    strShiftInfo = strShifts.Split("*")(n)
                    If strShiftInfo.Split("~").Length = 2 Then
                        strStart = strShiftInfo.Split("~")(1)
                        If strStart.Length >= 12 Then
                            Ret(n) = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
                        End If
                    End If
                End If
            Next

            Return Ret

        End Function

        Public Shared Function ParseIDAssignment(ByVal strIDShifts As String) As Integer

            Dim intRet As Integer = 0

            If strIDShifts.Split("*").Length > 5 Then
                Dim strID As String = strIDShifts.Split("*")(5)
                If IsNumeric(strID) Then
                    intRet = Val(strID)
                End If
            End If

            Return intRet

        End Function

        Public Shared Function ParseIDAssignmentBase(ByVal strIDShifts As String) As Integer

            Dim intRet As Integer = 0

            If strIDShifts.Split("*").Length > 6 Then
                Dim strID As String = strIDShifts.Split("*")(6)
                If IsNumeric(strID) Then
                    intRet = Val(strID)
                End If
            End If

            Return intRet

        End Function

        Public Shared Function ParseIsHolidays(ByVal strIDShifts As String) As Boolean

            Dim intRet As Boolean = False

            If strIDShifts.Split("*").Length > 7 Then
                Dim strID As String = strIDShifts.Split("*")(7)
                If IsNumeric(strID) Then
                    intRet = IIf(Val(strID) = 0, False, True)
                End If
            End If

            Return intRet

        End Function

        ''' <summary>
        ''' Planifica la lista de horarios a un empleado entre fechas.<br></br>
        ''' Si es necesario, la lista de horarios se asigna de forma cíclica.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
        ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.
        ''' </summary>
        ''' <param name="lstShifts">Lista de horarios y puesto (IDShift1*IDShift2*IDShift3*IDShift4*IDAssignment). Opcionalmente se puede informar de la hora de inicio del horario flotante  (IDShift1~yyyyMMddHHmm*IDShift2~yyyyMMddHHmm*IDShift3~yyyyMMddHHmm*IDShift4~yyyyMMddHHmm)</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha final de planificación.</param>
        ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyPlan(ByVal lstShifts As Generic.List(Of String), ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                 ByVal intShiftType As ActionShiftType, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction,
                                 ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByRef oState As Employee.roEmployeeState, Optional ByVal bAudit As Boolean = False,
                                 Optional ByVal _ShiftPermissionAction As ShiftPermissionAction = ShiftPermissionAction.ContinueAll, Optional ByVal sRequestFeaturePermission As String = "") As Boolean

            Dim bolRet As Boolean = False
            Dim bolTmp As Boolean = False
            Dim bolNotify As Boolean = False
            oState.UpdateStateInfo()

            Try

                Dim bHasPermission As Boolean = False
                bHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intDestinationIDEmployee, xBeginDate, )

                If sRequestFeaturePermission <> String.Empty AndAlso bHasPermission = False Then
                    bHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, sRequestFeaturePermission, Permission.Write, intDestinationIDEmployee, xBeginDate, )
                End If

                ' Verificamos los permisos del pasaporte actual sobre la planificación
                If bHasPermission Then

                    Dim intShiftsIndex As Integer = 0
                    Dim Shifts() As Integer = {0, 0, 0, 0, 0}
                    Dim StartShifts() As DateTime = {Nothing, Nothing, Nothing, Nothing, Nothing}
                    Dim intIDAssignment As Integer = 0
                    Dim intIDAssignmentBase As Integer = 0
                    Dim bIsHolidays As Boolean = False

                    Dim xDate As Date = xBeginDate

                    ' Verificamos que si se informa una fecha de bloqueo 'xDateLocked' sea dentro del periodo de copia
                    If xDateLocked <> Nothing AndAlso (xDateLocked < xDate OrElse xDateLocked > xEndDate) Then
                        xDateLocked = Nothing
                    End If

                    While xDate <= xEndDate

                        If intShiftsIndex >= lstShifts.Count Then intShiftsIndex = 0

                        If xDateLocked = Nothing OrElse (xDateLocked <> Nothing AndAlso xDateLocked = xDate) Then

                            Dim intIDShiftBase As Integer = -1
                            Dim xStartShiftBase As DateTime = Nothing
                            Dim intNewIDAssignmentBase As Integer = -1
                            Dim bNewIsHolidays As Boolean = False

                            xDateLocked = Nothing

                            ' Obtenemos los ids de los horarios a planificar para ese día
                            Shifts = roScheduler.ParseIDShifts(lstShifts(intShiftsIndex))
                            StartShifts = roScheduler.ParseStartShifts(lstShifts(intShiftsIndex))
                            intIDAssignment = roScheduler.ParseIDAssignment(lstShifts(intShiftsIndex))
                            intIDAssignmentBase = roScheduler.ParseIDAssignmentBase(lstShifts(intShiftsIndex))
                            bIsHolidays = roScheduler.ParseIsHolidays(lstShifts(intShiftsIndex))
                            If intShiftType = ActionShiftType.PrimaryShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) Then
                                    If copyHolidays Then
                                        intIDShiftBase = Shifts(4)
                                        xStartShiftBase = StartShifts(4)
                                        intNewIDAssignmentBase = intIDAssignmentBase
                                        bNewIsHolidays = bIsHolidays
                                    Else
                                        Shifts(0) = Shifts(4)
                                        StartShifts(0) = StartShifts(4)
                                        intIDAssignment = intIDAssignmentBase

                                        intIDShiftBase = -1
                                        xStartShiftBase = Nothing
                                        intNewIDAssignmentBase = -1
                                        bNewIsHolidays = False
                                    End If

                                End If

                                Shifts(1) = 0
                                Shifts(2) = 0
                                Shifts(3) = 0
                                StartShifts(1) = Nothing
                                StartShifts(2) = Nothing
                                StartShifts(3) = Nothing
                            ElseIf intShiftType = ActionShiftType.AlterShift Then
                                Shifts(0) = 0
                                StartShifts(0) = Nothing
                                intIDAssignment = 0
                            ElseIf intShiftType = ActionShiftType.AllShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) Then
                                    If copyHolidays Then
                                        intIDShiftBase = Shifts(4)
                                        xStartShiftBase = StartShifts(4)
                                        intNewIDAssignmentBase = intIDAssignmentBase
                                        bNewIsHolidays = bIsHolidays
                                    Else
                                        Shifts(0) = Shifts(4)
                                        StartShifts(0) = StartShifts(4)
                                        intIDAssignment = intIDAssignmentBase

                                        intIDShiftBase = -1
                                        xStartShiftBase = Nothing
                                        intNewIDAssignmentBase = -1
                                        bNewIsHolidays = False
                                    End If

                                End If
                            ElseIf intShiftType = ActionShiftType.HolidayShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) = False Then
                                    Shifts(0) = 0
                                    StartShifts(0) = Nothing
                                    intIDAssignment = 0
                                End If

                                Shifts(1) = 0
                                Shifts(2) = 0
                                Shifts(3) = 0
                                StartShifts(1) = Nothing
                                StartShifts(2) = Nothing
                                StartShifts(3) = Nothing
                            End If

                            If Shifts(0) <> 0 OrElse Shifts(1) <> 0 OrElse Shifts(2) <> 0 OrElse Shifts(3) <> 0 Then
                                'Asignamos la planificación
                                If copyHolidays Then
                                    bolTmp = AssignShift(intDestinationIDEmployee, xDate, Shifts(0), Shifts(1), Shifts(2), Shifts(3), StartShifts(0), StartShifts(1), StartShifts(2), StartShifts(3), intIDAssignment,
                                                      _LockedDayAction, _CoverageDayAction, oState, True, False, , _ShiftPermissionAction, intIDShiftBase, intNewIDAssignmentBase, xStartShiftBase, bNewIsHolidays, sRequestFeaturePermission)
                                Else
                                    bolTmp = AssignShift(intDestinationIDEmployee, xDate, Shifts(0), Shifts(1), Shifts(2), Shifts(3), StartShifts(0), StartShifts(1), StartShifts(2), StartShifts(3), intIDAssignment,
                                                      _LockedDayAction, _CoverageDayAction, oState, True, False, , _ShiftPermissionAction, , , , , sRequestFeaturePermission)
                                End If

                                If bolTmp Then
                                    bolNotify = True
                                End If

                                If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                    _ShiftPermissionAction = ShiftPermissionAction.None
                                End If

                                If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                   oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                   oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                    ' El día está bloqueado o hay una covertura O UN HORARIO SIN PERMISO. Obtenemos la fecha bloqueada y finalizamos proceso.
                                    xDateLocked = xDate
                                    Exit While
                                ElseIf oState.Result = EmployeeResultEnum.Exception Then
                                    Exit While
                                End If
                            End If

                        End If

                        intShiftsIndex += 1

                        xDate = xDate.AddDays(1)

                    End While

                    If bolNotify Then
                        ' Notificamos el cambio
                        roConnector.InitTask(TasksType.DAILYSCHEDULE)
                    End If
                Else
                    If _ShiftPermissionAction <> ShiftPermissionAction.ContinueAll Then
                        oState.Result = EmployeeResultEnum.AccessDenied
                    End If
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied Or
                          oState.Result = EmployeeResultEnum.NoWorkingDay)

                ' Auditamos consulta horario
                If bolRet And bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", roBusinessSupport.GetEmployeeName(intDestinationIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xBeginDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xEndDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCopyPlanificationToEmployees, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Planifica la lista de horarios a varios empleado entre fechas.<br></br>
        ''' La lista de horarios corresponde a los horarios a asignar para una misma fecha para los distintos empleados. Si es necesario, la lista de horarios se asigna de forma cíclica para una misma fecha. Si hay varios días a planificar, se repite la lista para cada día.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
        ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.<br></br>
        ''' La forma de asignar los horarios es la siguiente: por cada fecha del periodo se recorre todos los empleados a planificar y les asigna el/los horarios correspondientes.
        ''' </summary>
        ''' <param name="lstShifts">Lista de horarios y puesto opcional (IDShift1*IDShift2*IDShift3*IDShift4*IDAssignment). Opcionalmente se puede informar de la hora de inicio del horario flotante  (IDShift1~yyyyMMddHHmm*IDShift2~yyyyMMddHHmm*IDShift3~yyyyMMddHHmm*IDShift4~yyyyMMddHHmm).</param>
        ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha fin de planificación.</param>
        ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
        ''' <param name="oState">Información adicional de estado.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyPlan(ByVal lstShifts As Generic.List(Of String), ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                 ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByRef xDateLocked As Date, ByRef intIDEmployeeLocked As Integer, ByVal copyHolidays As Boolean, ByRef oState As Employee.roEmployeeState,
                                 Optional ByVal bAudit As Boolean = False, Optional ByVal bolKeepHolidays As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bolTemp As Boolean = False

            Try

                Dim intShiftsIndex As Integer = 0
                Dim Shifts() As Integer = {0, 0, 0, 0, 0}
                Dim StartShifts() As DateTime = {Nothing, Nothing, Nothing, Nothing, Nothing}
                Dim intIDAssignment As Integer = 0
                Dim intIDAssignmentBase As Integer = 0
                Dim bIsHolidays As Boolean = False

                Dim bolNotify As Boolean = False

                Dim xDate As Date = xBeginDate

                ' Verificamos que si se informa un empleado de bloqueo 'intIDEmployeeLocked' esté dentro del grupo de copia
                If intIDEmployeeLocked > 0 Then
                    Dim bolValidID As Boolean = False
                    For Each intID As Integer In lstDestionationIDEmployees
                        If intID = intIDEmployeeLocked Then
                            bolValidID = True
                            Exit For
                        End If
                    Next
                    If Not bolValidID Then intIDEmployeeLocked = 0
                End If

                ' Verificamos que si se informa una fecha de bloqueo 'xDateLocked' sea dentro del periodo de copia
                If xDateLocked <> Nothing AndAlso (xDateLocked < xDate OrElse xDateLocked > xEndDate) Then
                    xDateLocked = Nothing
                End If

                If xDateLocked <> Nothing Then
                    xDate = xDateLocked
                    xDateLocked = Nothing
                End If

                intShiftsIndex = 0
                While xDate <= xEndDate

                    For Each intIDEmployee As Integer In lstDestionationIDEmployees

                        If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 AndAlso intIDEmployeeLocked = intIDEmployee) Then
                            Dim intIDShiftBase As Integer = -1
                            Dim xStartShiftBase As DateTime = Nothing
                            Dim intNewIDAssignmentBase As Integer = -1
                            Dim bNewIsHolidays As Boolean = False

                            intIDEmployeeLocked = 0

                            ' Obtenemos los ids de los horarios a planificar para ese día
                            If intShiftsIndex >= lstShifts.Count Then intShiftsIndex = 0
                            Shifts = roScheduler.ParseIDShifts(lstShifts(intShiftsIndex))
                            StartShifts = roScheduler.ParseStartShifts(lstShifts(intShiftsIndex))
                            intIDAssignment = roScheduler.ParseIDAssignment(lstShifts(intShiftsIndex))
                            intIDAssignmentBase = roScheduler.ParseIDAssignmentBase(lstShifts(intShiftsIndex))
                            bIsHolidays = roScheduler.ParseIsHolidays(lstShifts(intShiftsIndex))
                            If intShiftType = ActionShiftType.PrimaryShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) Then
                                    If copyHolidays Then
                                        intIDShiftBase = Shifts(4)
                                        xStartShiftBase = StartShifts(4)
                                        intNewIDAssignmentBase = intIDAssignmentBase
                                        bNewIsHolidays = bIsHolidays
                                    Else
                                        Shifts(0) = Shifts(4)
                                        StartShifts(0) = StartShifts(4)
                                        intIDAssignment = intIDAssignmentBase

                                        intIDShiftBase = -1
                                        xStartShiftBase = Nothing
                                        intNewIDAssignmentBase = -1
                                        bNewIsHolidays = False
                                    End If

                                End If

                                Shifts(1) = 0
                                Shifts(2) = 0
                                Shifts(3) = 0
                                StartShifts(1) = Nothing
                                StartShifts(2) = Nothing
                                StartShifts(3) = Nothing
                            ElseIf intShiftType = ActionShiftType.AlterShift Then
                                Shifts(0) = 0
                                StartShifts(0) = Nothing
                                intIDAssignment = 0
                            ElseIf intShiftType = ActionShiftType.AllShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) Then
                                    If copyHolidays Then
                                        intIDShiftBase = Shifts(4)
                                        xStartShiftBase = StartShifts(4)
                                        intNewIDAssignmentBase = intIDAssignmentBase
                                        bNewIsHolidays = bIsHolidays
                                    Else
                                        Shifts(0) = Shifts(4)
                                        StartShifts(0) = StartShifts(4)
                                        intIDAssignment = intIDAssignmentBase

                                        intIDShiftBase = -1
                                        xStartShiftBase = Nothing
                                        intNewIDAssignmentBase = -1
                                        bNewIsHolidays = False
                                    End If

                                End If
                            ElseIf intShiftType = ActionShiftType.HolidayShift Then
                                If Shift.roShift.IsHolidays(Shifts(0)) = False Then
                                    Shifts(0) = 0
                                    StartShifts(0) = Nothing
                                    intIDAssignment = 0
                                End If

                                Shifts(1) = 0
                                Shifts(2) = 0
                                Shifts(3) = 0
                                StartShifts(1) = Nothing
                                StartShifts(2) = Nothing
                                StartShifts(3) = Nothing
                            End If

                            If Shifts(0) <> 0 Or Shifts(1) <> 0 Or Shifts(2) <> 0 Or Shifts(3) <> 0 Then
                                If bolKeepHolidays = False Then
                                    AssignShift(intIDEmployee, xDate, -1, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, 0,
                                              _LockedDayAction, _CoverageDayAction, oState, True, False, , , _ShiftPermissionAction, , xStartShiftBase, bNewIsHolidays)
                                End If

                                'Asignamos la planificación
                                If copyHolidays Then
                                    bolTemp = AssignShift(intIDEmployee, xDate, Shifts(0), Shifts(1), Shifts(2), Shifts(3), StartShifts(0), StartShifts(1), StartShifts(2), StartShifts(3), intIDAssignment,
                                                      _LockedDayAction, _CoverageDayAction, oState, True, False, , _ShiftPermissionAction, intIDShiftBase, intNewIDAssignmentBase, xStartShiftBase, bNewIsHolidays)
                                Else
                                    bolTemp = AssignShift(intIDEmployee, xDate, Shifts(0), Shifts(1), Shifts(2), Shifts(3), StartShifts(0), StartShifts(1), StartShifts(2), StartShifts(3), intIDAssignment,
                                                      _LockedDayAction, _CoverageDayAction, oState, True, False, , _ShiftPermissionAction)
                                End If

                                If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                    _ShiftPermissionAction = ShiftPermissionAction.None
                                End If

                                If bolTemp Then
                                    bolNotify = True
                                End If
                                If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                   oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                   oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                    ' El día está bloqueado o hay una covertura. Obtenemos el código del empleado y fecha bloqueada, y finalizamos proceso.
                                    intIDEmployeeLocked = intIDEmployee
                                    xDateLocked = xDate
                                    Exit For
                                ElseIf oState.Result = EmployeeResultEnum.Exception Then
                                    Exit For
                                End If
                            End If

                        End If

                        intShiftsIndex += 1

                    Next

                    If oState.Result = EmployeeResultEnum.Exception Or
                       oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                       oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                        Exit While
                    End If

                    xDate = xDate.AddDays(1)

                End While

                If bolNotify Then
                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)

                ' Auditamos consulta horario
                If bolRet AndAlso bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()

                    ' Contruimos cadenas de nombres de empleado
                    Dim sDestinationEmployeeNames As String = roBusinessSupport.GetAuditEmployeeNames(lstDestionationIDEmployees, oState)

                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", sDestinationEmployeeNames, "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xBeginDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xEndDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCopyPlanificationToEmployees, "", tbParameters, -1)

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Asigna justificación de forma masiva a un empleado en un periodo de fechas
        ''' </summary>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se le realizará la asignación.</param>
        ''' <param name="xBeginPeriod">Fecha inicio del periodo de la asignación.</param>
        ''' <param name="xEndPeriod">Fecha fin del periodo de la asignación.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AssignCause(ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer, ByVal bCompletedDay As Boolean, ByVal xFreezingDate As Date, ByRef oState As Employee.roEmployeeState, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            oState.UpdateStateInfo()

            Try

                Dim strFeaturePermission As String = ""
                strFeaturePermission = IIf(bCompletedDay, "Calendar.JustifyIncidences", "Calendar.Punches.Punches")

                ' Verificamos los permisos del pasaporte actual sobre la feature concreta (Justificar incidencias o modificar fichajes)
                If WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, strFeaturePermission, Permission.Write, intDestinationIDEmployee, xBeginPeriod) Then

                    'Comprobamos si es de tipo completo o parcial
                    If bCompletedDay Then
                        'Justificamos masivamente dias con ausencia
                        MassCauseCompletePeriod(intDestinationIDEmployee, xBeginPeriod, xEndPeriod, intIDCause, oState, xFreezingDate, False, strFeaturePermission)
                    Else
                        'Justificamos masivamente los fichajes de entrada y salida dentro del periodo indicado
                        MassCausePartialPeriod(intDestinationIDEmployee, xBeginPeriod, xEndPeriod, intIDCause, oState, xFreezingDate, False, strFeaturePermission)
                    End If
                Else
                    oState.Result = EmployeeResultEnum.AccessDenied
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)

                If bolRet Then
                    If bCompletedDay Then
                        roConnector.InitTask(TasksType.DAILYCAUSES)
                    Else
                        roConnector.InitTask(TasksType.MOVES)
                    End If
                End If

                ' Auditamos asignacion justificacion masiva
                If bolRet And bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", roBusinessSupport.GetEmployeeName(intDestinationIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xBeginPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xEndPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{CompletedDays}", bCompletedDay.ToString, "", 1)

                    Dim oCauseState As New Cause.roCauseState()
                    oCauseState.IDPassport = oState.IDPassport
                    Dim oCause = New Cause.roCause(intIDCause, oCauseState)
                    oState.AddAuditParameter(tbParameters, "{Cause}", IIf(oCause Is Nothing, "", oCause.Name), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tAssignCause, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::AssignCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::AssignCause")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de un empleado a otro
        ''' </summary>
        ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se copiará la planifiación.</param>
        ''' <param name="xBeginPeriod">Fecha inicio del periodo de copia.</param>
        ''' <param name="xEndPeriod">Fecha fin del periodo de copia.</param>
        ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        '''  </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días que hay covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <param name="oState"></param>
        ''' <param name="copyHolidays">nos indica si debemos copiar las vacaciones o no de una planificación a la otra</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyPlan(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByRef xDateLocked As Date, ByRef oState As Employee.roEmployeeState, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean,
                                 Optional ByVal bolKeepHolidays As Boolean = True, Optional ByVal bolExecuteTask As Boolean = True) As Boolean
            '
            ' Ejecuta la copia de la planificación del empleado 'intSourceIDEmployee' al 'intDestinationIDEmployee'
            ' Parámetros: intSourceIDEmployee: empleado del que se leerá la planificación
            '             intDestinationIDEmployee: empleado al que se copiará la planificación
            '             xBeginPeriod: fecha de inicio del periodo de cópia
            '             xEndPeriod: fecha de final del periodo de copia
            '             intShiftType: 0- Copia sólo los horarios principales sin vacaciones
            '                           1- Copia sólo los horarios alternativos sin vacaciones
            '                           2- Copia todos los horarios sin vacaciones
            '                           3- Copia solo vacaciones
            '
            Dim bolRet As Boolean = False
            Dim strSQL As String

            oState.UpdateStateInfo()

            Try

                Dim oLicense As New roServerLicense
                Dim obolHRSchedulingInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                ' Verificamos los permisos del pasaporte actual sobre la planificación
                If WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intDestinationIDEmployee, xBeginPeriod) Then

                    ' Verificamos que si se informa una fecha de bloqueo 'xDateLocked' sea dentro del periodo de copia
                    If xDateLocked <> Nothing AndAlso (xDateLocked < xBeginPeriod OrElse xDateLocked > xEndPeriod) Then
                        xDateLocked = Nothing
                    End If

                    ' Si se ha informado una fecha de bloqueo válida, modificamos la fecha de inicio
                    If xDateLocked <> Nothing Then
                        xBeginPeriod = xDateLocked
                        xDateLocked = Nothing
                    End If

                    'Seleccionamos la planificación de todos los dias dentro
                    'del intervalo a copiar
                    strSQL = "@SELECT# IDShift1, IDShift2, IDShift3, IDShift4, IDShiftBase, Date, LockedDay, " &
                             "StartShift1, StartShift2, StartShift3, StartShift4, StartShiftBase, " &
                             "StartFlexible1, EndFlexible1, ShiftName1, ShiftColor1, " &
                             "StartFlexibleBase, EndFlexibleBase, ShiftNameBase, ShiftColorBase, " &
                             "IDAssignment, IDAssignmentBase, " &
                             "IsHolidays ,LayersDefinition, " &
                             "ExpectedWorkingHours " &
                             "From DailySchedule " &
                             "WHERE IDEmployee = " & intSourceIDEmployee & " AND " &
                             "Date BETWEEN " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND " & Any2Time(xEndPeriod).SQLSmallDateTime
                    Dim tb As DataTable = CreateDataTable(strSQL)

                    'Definicion de variables
                    Dim intIDShift1 As Integer = 0
                    Dim intIDShift2 As Integer = 0
                    Dim intIDShift3 As Integer = 0
                    Dim intIDShift4 As Integer = 0
                    Dim intIDShiftBase As Integer = 0
                    Dim xStartShift1 As DateTime = Nothing
                    Dim xStartShift2 As DateTime = Nothing
                    Dim xStartShift3 As DateTime = Nothing
                    Dim xStartShift4 As DateTime = Nothing
                    Dim xStartShiftBase As DateTime = Nothing
                    Dim intIDAssignment As Integer = 0
                    Dim intIDAssignmentBase As Integer = 0
                    Dim bIsHolidays As Boolean = False
                    'Starter
                    Dim xStartFlexible1 As DateTime = Nothing
                    Dim xEndFlexible1 As DateTime = Nothing
                    Dim xStartFlexibleBase As DateTime = Nothing
                    Dim xEndFlexibleBase As DateTime = Nothing
                    Dim cColorFlexible1 As Integer = 0
                    Dim cColorFlexibleBase As Integer = 0
                    Dim sFlexibleShiftName1 As String = String.Empty
                    Dim sFlexibleShiftNameBase As String = String.Empty

                    Dim bolExistBusinessGroup As Boolean = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from ShiftGroups WHERE isnull(BusinessGroup, '') <> ''")) > 0)

                    If obolHRSchedulingInstalled Then
                        obolHRSchedulingInstalled = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from Assignments ")) > 0)
                    End If

                    For Each oRow As DataRow In tb.Rows

                        'Asignamos la planificación
                        If Not IsDBNull(oRow("IDShift1")) Then

                            intIDShift1 = 0
                            intIDShift2 = 0
                            intIDShift3 = 0
                            intIDShift4 = 0
                            intIDShiftBase = 0
                            xStartShift1 = Nothing
                            xStartShift2 = Nothing
                            xStartShift3 = Nothing
                            xStartShift4 = Nothing
                            xStartShiftBase = Nothing
                            intIDAssignment = 0
                            intIDAssignmentBase = 0
                            bIsHolidays = False
                            'Starter
                            xStartFlexible1 = Nothing
                            xEndFlexible1 = Nothing
                            xStartFlexibleBase = Nothing
                            xEndFlexibleBase = Nothing
                            cColorFlexible1 = 0
                            cColorFlexibleBase = 0
                            sFlexibleShiftName1 = String.Empty
                            sFlexibleShiftNameBase = String.Empty

                            If intShiftType = ActionShiftType.PrimaryShift Or intShiftType = ActionShiftType.AllShift Then
                                intIDShift1 = oRow("IDShift1")
                                If Not IsDBNull(oRow("StartShift1")) Then
                                    xStartShift1 = oRow("StartShift1")
                                End If
                                If Not IsDBNull(oRow("IDAssignment")) Then
                                    intIDAssignment = oRow("IDAssignment")
                                Else
                                    intIDAssignment = -1
                                End If
                                If Not IsDBNull(oRow("StartFlexible1")) Then
                                    xStartFlexible1 = oRow("StartFlexible1")
                                End If
                                If Not IsDBNull(oRow("EndFlexible1")) Then
                                    xEndFlexible1 = oRow("EndFlexible1")
                                End If
                                If Not IsDBNull(oRow("ShiftName1")) Then
                                    sFlexibleShiftName1 = oRow("ShiftName1")
                                End If
                                If Not IsDBNull(oRow("ShiftColor1")) Then
                                    cColorFlexible1 = oRow("ShiftColor1")
                                End If

                                If Shift.roShift.IsHolidays(roTypes.Any2Integer(oRow("IDShift1"))) Then
                                    If copyHolidays Then
                                        If Not IsDBNull(oRow("IDShiftBase")) Then
                                            intIDShiftBase = oRow("IDShiftBase")
                                            If Not IsDBNull(oRow("StartShiftBase")) Then
                                                xStartShiftBase = oRow("StartShiftBase")
                                            End If

                                            If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                xStartFlexibleBase = oRow("StartFlexibleBase")
                                            End If
                                            If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                xEndFlexibleBase = oRow("EndFlexibleBase")
                                            End If
                                            If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                sFlexibleShiftNameBase = oRow("ShiftNameBase")
                                            End If
                                            If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                cColorFlexibleBase = oRow("ShiftColorBase")
                                            End If
                                        Else
                                            intIDShiftBase = -1
                                            xStartShiftBase = Nothing
                                        End If
                                        If Not IsDBNull(oRow("IDAssignmentBase")) Then
                                            intIDAssignmentBase = oRow("IDAssignmentBase")
                                        Else
                                            intIDAssignmentBase = -1
                                        End If
                                        If Not IsDBNull(oRow("IsHolidays")) Then
                                            bIsHolidays = roTypes.Any2Boolean(oRow("IsHolidays"))
                                        Else
                                            bIsHolidays = False
                                        End If
                                    Else
                                        If Not IsDBNull(oRow("IDShiftBase")) Then
                                            intIDShift1 = oRow("IDShiftBase")
                                            If Not IsDBNull(oRow("StartShiftBase")) Then
                                                xStartShift1 = oRow("StartShiftBase")
                                            End If

                                            If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                xStartFlexible1 = oRow("StartFlexibleBase")
                                            End If
                                            If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                xEndFlexible1 = oRow("EndFlexibleBase")
                                            End If
                                            If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                sFlexibleShiftName1 = oRow("ShiftNameBase")
                                            End If
                                            If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                cColorFlexible1 = oRow("ShiftColorBase")
                                            End If
                                        Else
                                            intIDShift1 = -1
                                            xStartShift1 = Nothing
                                        End If
                                        If Not IsDBNull(oRow("IDAssignmentBase")) Then
                                            intIDAssignment = oRow("IDAssignmentBase")
                                        Else
                                            intIDAssignment = -1
                                        End If

                                        intIDShiftBase = -1
                                        'TODO Limpiar Base de Flexible Starter
                                        xStartShiftBase = Nothing
                                        intIDAssignment = -1
                                        bIsHolidays = False
                                    End If
                                End If
                            End If
                            If intShiftType = ActionShiftType.AlterShift Or intShiftType = ActionShiftType.AllShift Then
                                If Not IsDBNull(oRow("IDShift2")) Then
                                    intIDShift2 = oRow("IDShift2")
                                    If Not IsDBNull(oRow("StartShift2")) Then
                                        xStartShift2 = oRow("StartShift2")
                                    End If
                                Else
                                    intIDShift2 = -1
                                End If
                                If Not IsDBNull(oRow("IDShift3")) Then
                                    intIDShift3 = oRow("IDShift3")
                                    If Not IsDBNull(oRow("StartShift3")) Then
                                        xStartShift3 = oRow("StartShift3")
                                    End If
                                Else
                                    intIDShift3 = -1
                                End If
                                If Not IsDBNull(oRow("IDShift4")) Then
                                    intIDShift4 = oRow("IDShift4")
                                    If Not IsDBNull(oRow("StartShift4")) Then
                                        xStartShift4 = oRow("StartShift4")
                                    End If
                                Else
                                    intIDShift4 = -1
                                End If
                            End If

                            If intShiftType = ActionShiftType.HolidayShift Then
                                If Shift.roShift.IsHolidays(oRow("IDShift1")) = False Then
                                    intIDShift1 = 0
                                    xStartShift1 = Nothing
                                    intIDAssignment = 0
                                Else
                                    intIDShift1 = oRow("IDShift1")
                                    If Not IsDBNull(oRow("StartShift1")) Then
                                        xStartShift1 = oRow("StartShift1")
                                    End If

                                    If Not IsDBNull(oRow("StartFlexible1")) Then
                                        xStartFlexible1 = oRow("StartFlexible1")
                                    End If
                                    If Not IsDBNull(oRow("EndFlexible1")) Then
                                        xEndFlexible1 = oRow("EndFlexible1")
                                    End If
                                    If Not IsDBNull(oRow("ShiftName1")) Then
                                        sFlexibleShiftName1 = oRow("ShiftName1")
                                    End If
                                    If Not IsDBNull(oRow("ShiftColor1")) Then
                                        cColorFlexible1 = oRow("ShiftColor1")
                                    End If

                                    If Not IsDBNull(oRow("IDAssignment")) Then
                                        intIDAssignment = oRow("IDAssignment")
                                    Else
                                        intIDAssignment = -1
                                    End If
                                End If

                                intIDShift2 = 0
                                intIDShift3 = 0
                                intIDShift4 = 0
                                xStartShift2 = Nothing
                                xStartShift3 = Nothing
                                xStartShift4 = Nothing
                            End If

                            If intIDShift1 <> 0 Or intIDShift2 <> 0 Or intIDShift3 <> 0 Or intIDShift4 <> 0 Then
                                Dim bolRetAssignShiftEx As Boolean = False
                                If bolKeepHolidays = False Then
                                    bolRetAssignShiftEx = AssignShiftEx(intDestinationIDEmployee, oRow("Date"), -1, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, 0,
                                              _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, False, ,  , _ShiftPermissionAction, , , xStartShiftBase, bIsHolidays,,,,, xStartFlexibleBase, xEndFlexibleBase, cColorFlexibleBase, sFlexibleShiftNameBase)
                                End If

                                If copyHolidays Then
                                    bolRetAssignShiftEx = AssignShiftEx(intDestinationIDEmployee, oRow("Date"), intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                              _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, False, ,  , _ShiftPermissionAction, intIDShiftBase, intIDAssignmentBase, xStartShiftBase, bIsHolidays, xStartFlexible1, xEndFlexible1, cColorFlexible1, sFlexibleShiftName1, xStartFlexibleBase, xEndFlexibleBase, cColorFlexibleBase, sFlexibleShiftNameBase)
                                Else
                                    'Si esta marcado vacaciones y el horario tiene un base definido, este pasa a ser el horario principal
                                    bolRetAssignShiftEx = AssignShiftEx(intDestinationIDEmployee, oRow("Date"), intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                              _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, False, ,  , _ShiftPermissionAction,,,,, xStartFlexible1, xEndFlexible1, cColorFlexible1, sFlexibleShiftName1)
                                End If

                                If bolRetAssignShiftEx And intShiftType <> ActionShiftType.HolidayShift Then
                                    ' En el caso que se haya planificado correctamente y tengamos que copiar los horarios principales o el base,
                                    ' copiamos los datos de franjas flotantes y/o complementarias en caso necesario
                                    ' No copiamos los datos si nos han marcado que únicamente quieren copiar el horario de vacaciones
                                    Dim dblExpectedWorkingHours As Double = -1
                                    If Not IsDBNull(oRow("ExpectedWorkingHours")) Then dblExpectedWorkingHours = oRow("ExpectedWorkingHours")

                                    Dim strLayersDefinition As String = ""
                                    If Not IsDBNull(oRow("LayersDefinition")) Then strLayersDefinition = oRow("LayersDefinition")
                                    AssignShiftData(intDestinationIDEmployee, oRow("Date"), strLayersDefinition, dblExpectedWorkingHours, _LockedDayAction, oState)
                                End If

                                If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                    _ShiftPermissionAction = ShiftPermissionAction.None
                                End If
                            End If
                        Else
                            'Quitamos la planificación
                            Dim intShiftPosition As Integer = 1
                            Select Case intShiftType
                                Case ActionShiftType.AllShift
                                    RemoveShift(-1, intDestinationIDEmployee, oRow("Date"), _LockedDayAction, _CoverageDayAction, oState, False, False)
                                Case ActionShiftType.PrimaryShift
                                    RemoveShift(1, intDestinationIDEmployee, oRow("Date"), _LockedDayAction, _CoverageDayAction, oState, False, False)
                                Case ActionShiftType.AlterShift
                                    RemoveShift(2, intDestinationIDEmployee, oRow("Date"), _LockedDayAction, _CoverageDayAction, oState, False, False)
                                    RemoveShift(3, intDestinationIDEmployee, oRow("Date"), _LockedDayAction, _CoverageDayAction, oState, False, False)
                                    RemoveShift(4, intDestinationIDEmployee, oRow("Date"), _LockedDayAction, _CoverageDayAction, oState, False, False)
                            End Select
                        End If

                        If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                           oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                           oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                            ' El día está bloqueado. Obtenemos la fecha bloqueada y finalizamos proceso.
                            xDateLocked = oRow("Date")
                            Exit For
                        End If

                    Next
                Else
                    If _ShiftPermissionAction <> ShiftPermissionAction.ContinueAll Then
                        oState.Result = EmployeeResultEnum.AccessDenied
                    End If
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)

                If bolRet And bolExecuteTask Then
                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If

                ' Auditamos planificación horario
                If bolRet And bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{SourceEmployeeName}", roBusinessSupport.GetEmployeeName(intSourceIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", roBusinessSupport.GetEmployeeName(intDestinationIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xBeginPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xEndPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCopyPlanification, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo origen y un fecha destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
        ''' </summary>
        ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
        ''' <param name="xSourceBeginPeriod">Fecha inicio del periodo a copiar.</param>
        ''' <param name="xSourceEndPeriod">Fecha fin del periodo a copiar.</param>
        ''' <param name="xDestinationBeginPeriod">Fecha inicio del periodo al que se copiará.</param>
        ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        '''  </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <param name="oState">Información adicional de estado.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyPlan(ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBeginPeriod As Date, ByVal xSourceEndPeriod As Date,
                                 ByVal xDestinationBeginPeriod As Date, ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByRef oState As Employee.roEmployeeState, Optional ByVal xDestinationEndPeriod As Date = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String

            oState.UpdateStateInfo()

            Try

                Dim oLicense As New roServerLicense
                Dim obolHRSchedulingInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                ' Verificamos los permisos del pasaporte actual sobre la planificación
                If WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intDestinationIDEmployee, xDestinationBeginPeriod) Then

                    Dim Shifts1 As New Generic.List(Of Integer)
                    Dim Shifts2 As New Generic.List(Of Integer)
                    Dim Shifts3 As New Generic.List(Of Integer)
                    Dim Shifts4 As New Generic.List(Of Integer)
                    Dim ShiftsBase As New Generic.List(Of Integer)

                    Dim StartShifts1 As New Generic.List(Of DateTime)
                    Dim StartShifts2 As New Generic.List(Of DateTime)
                    Dim StartShifts3 As New Generic.List(Of DateTime)
                    Dim StartShifts4 As New Generic.List(Of DateTime)
                    Dim StartShiftsBase As New Generic.List(Of DateTime)

                    Dim Assignments As New Generic.List(Of Integer)
                    Dim AssignmentsBase As New Generic.List(Of Integer)

                    Dim IsHolidays As New Generic.List(Of Boolean)

                    'Seleccionamos la planificación de todos los dias dentro del intervalo a copiar
                    strSQL = "@SELECT# IDShift1, IDShift2, IDShift3, IDShift4, IDShiftBase, Date, StartShift1, StartShift2, StartShift3, StartShift4, StartShiftBase, IDAssignment, IDAssignmentBase, IsHolidays FROM DailySchedule " &
                             "WHERE IDEmployee = " & intSourceIDEmployee & " AND " &
                             "Date BETWEEN " & Any2Time(xSourceBeginPeriod).SQLSmallDateTime & " AND " & Any2Time(xSourceEndPeriod).SQLSmallDateTime & " ORDER BY Date"
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    Dim oRows As DataRow()

                    Dim xDate As Date = xSourceBeginPeriod
                    While xDate <= xSourceEndPeriod

                        oRows = tb.Select("Date = '" & Format(xDate, "yyyy/MM/dd") & "'", "")
                        If oRows.Length = 1 Then

                            'copiamos la planificación en los arrays
                            If Not IsDBNull(oRows(0)("IDShift1")) Then

                                Shifts1.Add(roTypes.Any2Integer(oRows(0)("IDShift1")))
                                Shifts2.Add(roTypes.Any2Integer(oRows(0)("IDShift2")))
                                Shifts3.Add(roTypes.Any2Integer(oRows(0)("IDShift3")))
                                Shifts4.Add(roTypes.Any2Integer(oRows(0)("IDShift4")))
                                ShiftsBase.Add(roTypes.Any2Integer(oRows(0)("IDShiftBase")))
                                If Not IsDBNull(oRows(0)("StartShift1")) Then StartShifts1.Add(oRows(0)("StartShift1")) Else StartShifts1.Add(Nothing)
                                If Not IsDBNull(oRows(0)("StartShift2")) Then StartShifts2.Add(oRows(0)("StartShift2")) Else StartShifts2.Add(Nothing)
                                If Not IsDBNull(oRows(0)("StartShift3")) Then StartShifts3.Add(oRows(0)("StartShift3")) Else StartShifts3.Add(Nothing)
                                If Not IsDBNull(oRows(0)("StartShift4")) Then StartShifts4.Add(oRows(0)("StartShift4")) Else StartShifts4.Add(Nothing)
                                If Not IsDBNull(oRows(0)("StartShiftBase")) Then StartShiftsBase.Add(oRows(0)("StartShiftBase")) Else StartShiftsBase.Add(Nothing)
                                Assignments.Add(roTypes.Any2Integer(oRows(0)("IDAssignment")))
                                AssignmentsBase.Add(roTypes.Any2Integer(oRows(0)("IDAssignmentBase")))
                                IsHolidays.Add(roTypes.Any2Boolean(oRows(0)("IsHolidays")))
                            Else
                                ' Indicamos que se tiene que quitar la planificación para ese dia
                                Shifts1.Add(-1)
                                Shifts2.Add(-1)
                                Shifts3.Add(-1)
                                Shifts4.Add(-1)
                                ShiftsBase.Add(-1)
                                StartShifts1.Add(Nothing)
                                StartShifts2.Add(Nothing)
                                StartShifts3.Add(Nothing)
                                StartShifts4.Add(Nothing)
                                StartShiftsBase.Add(Nothing)
                                Assignments.Add(-1)
                                AssignmentsBase.Add(-1)
                                IsHolidays.Add(False)
                            End If
                        Else
                            ' Indicamos que se tiene que quitar la planificación para ese dia
                            Shifts1.Add(-2)
                            Shifts2.Add(-2)
                            Shifts3.Add(-2)
                            Shifts4.Add(-2)
                            ShiftsBase.Add(-2)
                            StartShifts1.Add(Nothing)
                            StartShifts2.Add(Nothing)
                            StartShifts3.Add(Nothing)
                            StartShifts4.Add(Nothing)
                            StartShiftsBase.Add(Nothing)
                            Assignments.Add(-1)
                            AssignmentsBase.Add(-1)
                            IsHolidays.Add(False)
                        End If

                        xDate = xDate.AddDays(1)

                    End While

                    xDate = xDestinationBeginPeriod
                    Dim xDateEnd As Date
                    If xDestinationEndPeriod = Nothing Then
                        Dim intDays As Integer = DateDiff(DateInterval.Day, xSourceBeginPeriod, xSourceEndPeriod)
                        xDateEnd = xDate.AddDays(intDays)
                    Else
                        xDateEnd = xDestinationEndPeriod
                    End If

                    ' Verificamos que si se informa una fecha de bloqueo 'xDateLocked' sea dentro del periodo de copia
                    If xDateLocked <> Nothing AndAlso (xDateLocked < xDate OrElse xDateLocked > xDateEnd) Then
                        xDateLocked = Nothing
                    End If

                    Dim n As Integer = 0

                    Dim intIDShift1 As Integer = 0
                    Dim intIDShift2 As Integer = 0
                    Dim intIDShift3 As Integer = 0
                    Dim intIDShift4 As Integer = 0
                    Dim intIDShiftBase As Integer = 0
                    Dim xStartShift1 As DateTime = Nothing
                    Dim xStartShift2 As DateTime = Nothing
                    Dim xStartShift3 As DateTime = Nothing
                    Dim xStartShift4 As DateTime = Nothing
                    Dim xStartShiftBase As DateTime = Nothing
                    Dim intIDAssignment As Integer = 0
                    Dim intIDAssignmentBase As Integer = 0
                    Dim bIsHolidays As Boolean = False

                    Dim bolExistBusinessGroup As Boolean = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from ShiftGroups WHERE isnull(BusinessGroup, '') <> ''")) > 0)

                    If obolHRSchedulingInstalled Then
                        obolHRSchedulingInstalled = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from Assignments ")) > 0)
                    End If

                    While xDate <= xDateEnd

                        If n >= Shifts1.Count Then n = 0

                        If xDateLocked = Nothing OrElse (xDateLocked <> Nothing AndAlso xDateLocked = xDate) Then

                            xDateLocked = Nothing

                            If Shifts1(n) >= 0 Then

                                intIDShift1 = 0
                                intIDShift2 = 0
                                intIDShift3 = 0
                                intIDShift4 = 0
                                intIDShiftBase = 0
                                xStartShift1 = Nothing
                                xStartShift2 = Nothing
                                xStartShift3 = Nothing
                                xStartShift4 = Nothing
                                xStartShiftBase = Nothing
                                intIDAssignment = 0
                                intIDAssignmentBase = 0
                                bIsHolidays = False

                                If intShiftType = ActionShiftType.PrimaryShift Or intShiftType = ActionShiftType.AllShift Then
                                    intIDShift1 = Shifts1(n)
                                    xStartShift1 = StartShifts1(n)
                                    intIDAssignment = Assignments(n)

                                    If Shift.roShift.IsHolidays(roTypes.Any2Integer(Shifts1(n))) Then
                                        If copyHolidays Then
                                            intIDShiftBase = ShiftsBase(n)
                                            xStartShiftBase = StartShiftsBase(n)
                                            intIDAssignmentBase = AssignmentsBase(n)
                                            bIsHolidays = IsHolidays(n)
                                        Else
                                            intIDShift1 = ShiftsBase(n)
                                            xStartShift1 = StartShiftsBase(n)
                                            intIDAssignment = AssignmentsBase(n)

                                            intIDShiftBase = -1
                                            xStartShiftBase = Nothing
                                            intIDAssignment = -1
                                            bIsHolidays = False
                                        End If
                                    End If
                                End If
                                If intShiftType = ActionShiftType.AlterShift Or intShiftType = ActionShiftType.AllShift Then
                                    intIDShift2 = Shifts2(n)
                                    intIDShift3 = Shifts3(n)
                                    intIDShift4 = Shifts4(n)
                                    xStartShift2 = StartShifts2(n)
                                    xStartShift3 = StartShifts3(n)
                                    xStartShift4 = StartShifts4(n)
                                End If

                                If intShiftType = ActionShiftType.HolidayShift Then
                                    If Shift.roShift.IsHolidays(Shifts1(n)) = False Then
                                        intIDShift1 = 0
                                        xStartShift1 = Nothing
                                        intIDAssignment = 0
                                    Else
                                        intIDShift1 = Shifts1(n)
                                        xStartShift1 = StartShifts1(n)
                                        intIDAssignment = Assignments(n)
                                    End If

                                    intIDShift2 = 0
                                    intIDShift3 = 0
                                    intIDShift4 = 0
                                    xStartShift2 = Nothing
                                    xStartShift3 = Nothing
                                    xStartShift4 = Nothing
                                End If

                                If intIDShift1 <> 0 Or intIDShift2 <> 0 Or intIDShift3 <> 0 Or intIDShift4 <> 0 Then
                                    If copyHolidays Then
                                        AssignShiftEx(intDestinationIDEmployee, xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                  _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, False, , , _ShiftPermissionAction, intIDShiftBase, intIDAssignmentBase, xStartShiftBase, bIsHolidays)
                                    Else
                                        AssignShiftEx(intDestinationIDEmployee, xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                  _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezingDate, bolExistBusinessGroup, False, , , _ShiftPermissionAction)
                                    End If

                                    If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                        _ShiftPermissionAction = ShiftPermissionAction.None
                                    End If
                                End If

                            ElseIf Shifts1(n) = -1 Then
                                'Quitamos la planificación
                                RemoveShift(1, intDestinationIDEmployee, xDate, _LockedDayAction, _CoverageDayAction, oState, False)
                            End If

                            If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                               oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                               oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                ' El día está bloqueado o hay covertura. Obtenemos la fecha bloqueada y finalizamos proceso.
                                xDateLocked = xDate
                                Exit While
                            End If

                        End If

                        xDate = xDate.AddDays(1)
                        n += 1

                    End While
                Else
                    If _ShiftPermissionAction <> ShiftPermissionAction.ContinueAll Then
                        oState.Result = EmployeeResultEnum.AccessDenied
                    End If
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)

                If bolRet Then
                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)

                    'If obolHRSchedulingInstalled Then
                    '    Dim Command(-1) As String
                    '    ReDim Command(1)
                    '    Command(0) = "UPDATE_PLANNED"
                    '    Command(1) = "UPDATE_ACTUAL"

                    '    Dim oParamsAux As roCollection = Nothing
                    '    For Each strCommand As String In Command
                    '        oParamsAux = New roCollection
                    '        oParamsAux.Add("Command", strCommand)
                    '        roConnector.InitTask(TasksType.HRSCHEDULER, Nothing, oParamsAux)
                    '    Next

                    'End If

                End If

                ' Auditamos
                If bolRet And bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{SourceEmployeeName}", roBusinessSupport.GetEmployeeName(intSourceIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", roBusinessSupport.GetEmployeeName(intDestinationIDEmployee, Nothing), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xSourceBeginPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xSourceEndPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationDateIni}", xDestinationBeginPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationDateEnd}", xDestinationEndPeriod.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCopyPlanificationAdvanced, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de unos empleados a otros. Indicando una fecha origen y otra destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' </summary>
        ''' <param name="SourceIDEmployees">Lista de códigos de los empleados origen.</param>
        ''' <param name="DestinationIDEmployees">Lista de códigos de los empleados destino.</param>
        ''' <param name="xSourceDate">Fecha origen.</param>
        ''' <param name="xDestinationDate">Fecha destino.</param>
        ''' <param name="intShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
        ''' <param name="oState">Información adicional de estado.</param>
        ''' <param name="xDestinationEndDate">Opcional. Fecha final periodo destino de copia.</param>
        ''' <param name="xDateLocked">Opcional. Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyPlan(ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date, ByVal intShiftType As ActionShiftType,
                                 ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                 ByRef intIDEmployeeLocked As Integer, ByVal copyHolidays As Boolean, ByRef oState As Employee.roEmployeeState, Optional ByVal xDestinationEndDate As Date = Nothing, Optional ByRef xDateLocked As Date = Nothing,
                                 Optional ByVal bAudit As Boolean = False, Optional ByVal bolKeepHolidays As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String

            oState.UpdateStateInfo()

            Try

                Dim Shifts1 As New Generic.List(Of Integer)
                Dim Shifts2 As New Generic.List(Of Integer)
                Dim Shifts3 As New Generic.List(Of Integer)
                Dim Shifts4 As New Generic.List(Of Integer)
                Dim ShiftsBase As New Generic.List(Of Integer)

                Dim StartShifts1 As New Generic.List(Of DateTime)
                Dim StartShifts2 As New Generic.List(Of DateTime)
                Dim StartShifts3 As New Generic.List(Of DateTime)
                Dim StartShifts4 As New Generic.List(Of DateTime)
                Dim StartShiftsBase As New Generic.List(Of DateTime)

                Dim Assignments As New Generic.List(Of Integer)
                Dim AssignmentsBase As New Generic.List(Of Integer)

                Dim IsHolidays As New Generic.List(Of Boolean)

                Dim intIDShift1 As Integer = 0
                Dim intIDShift2 As Integer = 0
                Dim intIDShift3 As Integer = 0
                Dim intIDShift4 As Integer = 0
                Dim intIDShiftBase As Integer = 0
                Dim xStartShift1 As DateTime = Nothing
                Dim xStartShift2 As DateTime = Nothing
                Dim xStartShift3 As DateTime = Nothing
                Dim xStartShift4 As DateTime = Nothing
                Dim xStartShiftBase As DateTime = Nothing
                Dim intIDAssignment As Integer = 0
                Dim intIDAssignmentBase As Integer = 0
                Dim bIsHolidays As Boolean = False

                Dim strIDEmployees As String = ""
                For Each strID As String In SourceIDEmployees
                    strIDEmployees &= strID & ","
                Next
                If strIDEmployees <> "" Then strIDEmployees = strIDEmployees.Substring(0, strIDEmployees.Length - 1)

                'Seleccionamos la planificación de todos los dias dentro del intervalo a copiar
                strSQL = "@SELECT# IDEmployee, IDShift1, IDShift2, IDShift3, IDShift4, IDShiftBase, Date, StartShift1, StartShift2, StartShift3, StartShift4, StartShiftBase, IDAssignment, IDAssignmentBase, IsHolidays From DailySchedule " &
                         "WHERE IDEmployee IN (" & strIDEmployees & ") AND Date = " & Any2Time(xSourceDate).SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL)

                ' Recorremos los empleados origen (respetando el orden) y inertamos horarios a los arrays.
                Dim oPlan() As DataRow
                Dim oRow As DataRow
                For Each strID As String In SourceIDEmployees

                    oPlan = tb.Select("IDEmployee = " & strID)
                    If oPlan.Length = 1 Then

                        oRow = oPlan(0)

                        'copiamos la planificación en los arrays
                        If Not IsDBNull(oRow("IDShift1")) Then

                            intIDShift1 = 0
                            intIDShift2 = 0
                            intIDShift3 = 0
                            intIDShift4 = 0
                            intIDShiftBase = 0
                            xStartShift1 = Nothing
                            xStartShift2 = Nothing
                            xStartShift3 = Nothing
                            xStartShift4 = Nothing
                            xStartShiftBase = Nothing
                            intIDAssignment = 0
                            intIDAssignmentBase = 0
                            bIsHolidays = False

                            intIDShift1 = oRow("IDShift1")
                            If Not IsDBNull(oRow("IDShift2")) Then intIDShift2 = oRow("IDShift2")
                            If Not IsDBNull(oRow("IDShift3")) Then intIDShift3 = oRow("IDShift3")
                            If Not IsDBNull(oRow("IDShift4")) Then intIDShift4 = oRow("IDShift4")
                            If Not IsDBNull(oRow("IDShiftBase")) Then intIDShiftBase = oRow("IDShiftBase")
                            Shifts1.Add(intIDShift1)
                            Shifts2.Add(intIDShift2)
                            Shifts3.Add(intIDShift3)
                            Shifts4.Add(intIDShift4)
                            ShiftsBase.Add(intIDShiftBase)
                            If Not IsDBNull(oRow("StartShift1")) Then xStartShift1 = oRow("StartShift1")
                            If Not IsDBNull(oRow("StartShift2")) Then xStartShift2 = oRow("StartShift2")
                            If Not IsDBNull(oRow("StartShift3")) Then xStartShift3 = oRow("StartShift3")
                            If Not IsDBNull(oRow("StartShift4")) Then xStartShift4 = oRow("StartShift4")
                            If Not IsDBNull(oRow("StartShiftBase")) Then xStartShiftBase = oRow("StartShiftBase")
                            StartShifts1.Add(xStartShift1)
                            StartShifts2.Add(xStartShift2)
                            StartShifts3.Add(xStartShift3)
                            StartShifts4.Add(xStartShift4)
                            StartShiftsBase.Add(xStartShiftBase)
                            If Not IsDBNull(oRow("IDAssignment")) Then intIDAssignment = oRow("IDAssignment")
                            If Not IsDBNull(oRow("IDAssignmentBase")) Then intIDAssignment = oRow("IDAssignmentBase")
                            Assignments.Add(intIDAssignment)
                            AssignmentsBase.Add(intIDAssignmentBase)

                            If Not IsDBNull(oRow("IsHolidays")) Then bIsHolidays = oRow("IsHolidays")
                            IsHolidays.Add(bIsHolidays)
                        Else
                            ' Indicamos que se tiene que quitar la planificación para ese empleado
                            Shifts1.Add(-1)
                            Shifts2.Add(-1)
                            Shifts3.Add(-1)
                            Shifts4.Add(-1)
                            ShiftsBase.Add(-1)
                            StartShifts1.Add(Nothing)
                            StartShifts2.Add(Nothing)
                            StartShifts3.Add(Nothing)
                            StartShifts4.Add(Nothing)
                            StartShiftsBase.Add(Nothing)
                            Assignments.Add(-1)
                            AssignmentsBase.Add(-1)
                            IsHolidays.Add(False)
                        End If
                    Else
                        ' Indicamos que no existe el registro de planificación para ese empleado
                        Shifts1.Add(-2)
                        Shifts2.Add(-2)
                        Shifts3.Add(-2)
                        Shifts4.Add(-2)
                        ShiftsBase.Add(-2)
                        StartShifts1.Add(Nothing)
                        StartShifts2.Add(Nothing)
                        StartShifts3.Add(Nothing)
                        StartShifts4.Add(Nothing)
                        StartShiftsBase.Add(Nothing)
                        Assignments.Add(-1)
                        AssignmentsBase.Add(-1)
                        IsHolidays.Add(False)
                    End If
                Next

                ' Verificamos que si se informa un empleado de bloqueo 'intIDEmployeeLocked' esté dentro del grupo de copia
                If intIDEmployeeLocked > 0 Then
                    Dim bolValidID As Boolean = False
                    For Each intID As Integer In DestinationIDEmployees
                        If intID = intIDEmployeeLocked Then
                            bolValidID = True
                            Exit For
                        End If
                    Next
                    If Not bolValidID Then intIDEmployeeLocked = 0
                End If

                ' Verificamos que si se informa una fecha de bloqueo 'xDateLocked' sea dentro del periodo de copia
                If xDateLocked <> Nothing AndAlso xDestinationEndDate <> Nothing AndAlso (xDateLocked < xDestinationDate OrElse xDateLocked > xDestinationEndDate) Then
                    xDateLocked = Nothing
                End If

                Dim bolNotify As Boolean = False
                Dim bolTemp As Boolean = False

                For n As Integer = 0 To SourceIDEmployees.Length - 1

                    ' Inicializamos el estado actual
                    oState.Result = EmployeeResultEnum.NoError

                    If n < DestinationIDEmployees.Length Then

                        If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 AndAlso intIDEmployeeLocked = DestinationIDEmployees(n)) Then

                            intIDEmployeeLocked = 0 ' Inicializamos id último empleado bloqueado.

                            If Shifts1(n) >= 0 Then

                                intIDShift1 = 0
                                intIDShift2 = 0
                                intIDShift3 = 0
                                intIDShift4 = 0
                                intIDShiftBase = 0
                                xStartShift1 = Nothing
                                xStartShift2 = Nothing
                                xStartShift3 = Nothing
                                xStartShift4 = Nothing
                                xStartShiftBase = Nothing
                                intIDAssignment = 0
                                intIDAssignmentBase = 0
                                bIsHolidays = False

                                If intShiftType = ActionShiftType.PrimaryShift Or intShiftType = ActionShiftType.AllShift Then
                                    intIDShift1 = Shifts1(n)
                                    xStartShift1 = StartShifts1(n)
                                    intIDAssignment = Assignments(n)

                                    If Shift.roShift.IsHolidays(roTypes.Any2Integer(Shifts1(n))) Then
                                        If copyHolidays Then
                                            intIDShiftBase = ShiftsBase(n)
                                            xStartShiftBase = StartShiftsBase(n)
                                            intIDAssignmentBase = AssignmentsBase(n)
                                            bIsHolidays = IsHolidays(n)
                                        Else
                                            intIDShift1 = ShiftsBase(n)
                                            xStartShift1 = StartShiftsBase(n)
                                            intIDAssignment = AssignmentsBase(n)

                                            intIDShiftBase = -1
                                            xStartShiftBase = Nothing
                                            intIDAssignment = -1
                                            bIsHolidays = False
                                        End If
                                    End If
                                End If
                                If intShiftType = ActionShiftType.AlterShift Or intShiftType = ActionShiftType.AllShift Then
                                    intIDShift2 = Shifts2(n)
                                    intIDShift3 = Shifts3(n)
                                    intIDShift4 = Shifts4(n)
                                    xStartShift2 = StartShifts2(n)
                                    xStartShift3 = StartShifts3(n)
                                    xStartShift4 = StartShifts4(n)
                                End If

                                If intShiftType = ActionShiftType.HolidayShift Then
                                    If Shift.roShift.IsHolidays(Shifts1(n)) = False Then
                                        intIDShift1 = 0
                                        xStartShift1 = Nothing
                                        intIDAssignment = 0
                                    Else
                                        intIDShift1 = Shifts1(n)
                                        xStartShift1 = StartShifts1(n)
                                        intIDAssignment = Assignments(n)
                                    End If

                                    intIDShift2 = 0
                                    intIDShift3 = 0
                                    intIDShift4 = 0
                                    xStartShift2 = Nothing
                                    xStartShift3 = Nothing
                                    xStartShift4 = Nothing
                                End If

                                If xDestinationEndDate = Nothing Then

                                    If intIDShift1 <> 0 Or intIDShift2 <> 0 Or intIDShift3 <> 0 Or intIDShift4 <> 0 Then
                                        If bolKeepHolidays = False Then
                                            bolTemp = AssignShift(DestinationIDEmployees(n), xDestinationDate, -1, 0, 0, 0, Nothing, Nothing, Nothing, Nothing,
                                                              0, _LockedDayAction, _CoverageDayAction, oState, , False, , _ShiftPermissionAction, , , xStartShiftBase, bIsHolidays)
                                        End If
                                        If copyHolidays Then
                                            bolTemp = AssignShift(DestinationIDEmployees(n), xDestinationDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                              intIDAssignment, _LockedDayAction, _CoverageDayAction, oState, , False,  , _ShiftPermissionAction, intIDShiftBase, intIDAssignmentBase, xStartShiftBase, bIsHolidays)
                                        Else
                                            bolTemp = AssignShift(DestinationIDEmployees(n), xDestinationDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                              intIDAssignment, _LockedDayAction, _CoverageDayAction, oState, , False,  , _ShiftPermissionAction)
                                        End If

                                        If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                            _ShiftPermissionAction = ShiftPermissionAction.None
                                        End If

                                        If bolTemp Then
                                            bolNotify = True
                                        End If

                                        If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                           oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                           oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                            ' El día está bloqueado o hay covertura. Obtenemos el id del empleado bloqueado y finalizamos proceso.
                                            intIDEmployeeLocked = DestinationIDEmployees(n)
                                            Exit For
                                        End If
                                    End If
                                Else
                                    Dim xDate As Date = xDestinationDate
                                    If xDateLocked <> Nothing Then
                                        xDate = xDateLocked
                                        xDateLocked = Nothing ' Inicializamos la fecha del último bloqueo
                                    End If
                                    While xDate <= xDestinationEndDate

                                        If intIDShift1 <> 0 Or intIDShift2 <> 0 Or intIDShift3 <> 0 Or intIDShift4 <> 0 Then
                                            If copyHolidays Then
                                                bolTemp = AssignShift(DestinationIDEmployees(n), xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                       intIDAssignment, _LockedDayAction, _CoverageDayAction, oState, , False, , _ShiftPermissionAction, intIDShiftBase, intIDAssignmentBase, xStartShiftBase, bIsHolidays)
                                            Else
                                                bolTemp = AssignShift(DestinationIDEmployees(n), xDate, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                       intIDAssignment, _LockedDayAction, _CoverageDayAction, oState, , False, , _ShiftPermissionAction)
                                            End If

                                            If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                                _ShiftPermissionAction = ShiftPermissionAction.None
                                            End If

                                            If bolTemp Then
                                                bolNotify = True
                                            End If
                                            If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                               oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                               oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                                ' El día está bloqueado o hay covertura. Obtenemos el id del empleado y fecha bloqueada. Finalizamos proceso.
                                                intIDEmployeeLocked = DestinationIDEmployees(n)
                                                xDateLocked = xDate
                                                Exit For
                                            End If
                                        End If

                                        xDate = xDate.AddDays(1)
                                    End While
                                End If

                            ElseIf Shifts1(n) = -1 Then
                                'Quitamos la planificación
                                If xDestinationEndDate = Nothing Then
                                    If RemoveShift(1, DestinationIDEmployees(n), xDestinationDate, _LockedDayAction, _CoverageDayAction, oState, , False) Then
                                        bolNotify = True
                                    End If
                                    If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                       oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                        ' El día está bloqueado o hay covertura. Obtenemos el id del empleado bloqueado. Finalizamos proceso.
                                        intIDEmployeeLocked = DestinationIDEmployees(n)
                                        Exit For
                                    End If
                                Else
                                    Dim xDate As Date = xDestinationDate
                                    If xDateLocked <> Nothing Then
                                        xDate = xDateLocked
                                        xDateLocked = Nothing ' Inicializamos la fecha del último bloqueo
                                    End If
                                    While xDate <= xDestinationEndDate
                                        If RemoveShift(1, DestinationIDEmployees(n), xDate, _LockedDayAction, _CoverageDayAction, oState, , False) Then
                                            bolNotify = True
                                        End If
                                        If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                           oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                            ' El día está bloqueado o hay covertura. Obtenemos el id del empleado y fecha bloqueada. Finalizamos proceso.
                                            intIDEmployeeLocked = DestinationIDEmployees(n)
                                            xDateLocked = xDate
                                            Exit For
                                        End If
                                        xDate = xDate.AddDays(1)
                                    End While
                                End If
                            End If

                        End If
                    Else
                        Exit For
                    End If

                Next

                If bolNotify Then
                    ' Notificamos el cambio
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)

                ' Auditamos consulta horario
                If bolRet And bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    ' Contruimos cadenas de nombres de empleado
                    Dim sSourceEmployeeNames As String = roBusinessSupport.GetAuditEmployeeNames(SourceIDEmployees.ToList, oState)
                    Dim sDestinationEmployeeNames As String = roBusinessSupport.GetAuditEmployeeNames(DestinationIDEmployees.ToList, oState)
                    oState.AddAuditParameter(tbParameters, "{SourceEmployeeName}", sSourceEmployeeNames, "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationEmployeeName}", sDestinationEmployeeNames, "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateIni}", xSourceDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DateEnd}", xSourceDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationDateIni}", xDestinationDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.AddAuditParameter(tbParameters, "{DestinationDateEnd}", xDestinationEndDate.ToString(oState.Language.GetShortDateFormat), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCopyPlanificationAdvanced, "", tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::CopyPlan")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Bloquea/desbloquea los días informados a un empleado entre fechas.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' </summary>
        ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha final de planificación.</param>
        ''' <param name="xLocked">Día bloqueado</param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LockDaysInPlan(ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByRef oState As Employee.roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            oState.UpdateStateInfo()

            Try

                ' Verificamos los permisos del pasaporte actual sobre la planificación
                If WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intDestinationIDEmployee) Then

                    Dim xDate As Date = xBeginDate

                    While xDate <= xEndDate

                        'Asignamos la planificación
                        LockDay(intDestinationIDEmployee, xDate, xLocked, oState, False)

                        If oState.Result = EmployeeResultEnum.Exception Then
                            Exit While
                        End If

                        xDate = xDate.AddDays(1)

                    End While
                Else
                    oState.Result = EmployeeResultEnum.AccessDenied
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::LockDaysInPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::LockDaysInPlan")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Bloquea/desbloquea los días informados varios empleado entre fechas.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' </summary>
        ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a bloquear / desbloquear.</param>
        ''' <param name="xBeginDate">Fecha inicio</param>
        ''' <param name="xEndDate">Fecha fin</param>
        ''' <param name="xLocked">Si se han de bloquear los días</param>
        ''' <param name="oState">Información adicional de estado.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LockDaysInPlan(ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean, ByRef oState As Employee.roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim intShiftsIndex As Integer = 0
                Dim Shifts() As Integer = {0, 0, 0, 0}

                Dim bolNotify As Boolean = False

                Dim xDate As Date = xBeginDate

                While xDate <= xEndDate

                    intShiftsIndex = 0

                    For Each intIDEmployee As Integer In lstDestionationIDEmployees

                        'Asignamos la planificación
                        If LockDay(intIDEmployee, xDate, xLocked, oState, True, False) Then
                            bolNotify = True
                        End If

                        intShiftsIndex += 1

                    Next

                    If oState.Result = EmployeeResultEnum.Exception Then
                        Exit While
                    End If

                    xDate = xDate.AddDays(1)

                End While

                bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                          oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                          oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                          oState.Result = EmployeeResultEnum.AccessDenied)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::LockDaysInPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::LockDaysInPlan")
            End Try

            Return bolRet

        End Function

        Public Shared Function LockDay(ByVal IDEmployee As Integer, ByVal xShiftDate As Date, ByVal xLock As Boolean, ByRef oState As Employee.roEmployeeState, Optional ByVal bolCheckPermission As Boolean = True, Optional ByVal bolNotify As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bolHasPermission As Boolean = True
            If bolCheckPermission Then
                ' Verificamos los permisos del pasaporte actual sobre la planificación
                bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, "Calendar.Scheduler", Permission.Write, IDEmployee)
            End If

            If bolHasPermission Then

                'Solamente asigna el horario si este tiene contrato
                If roBusinessSupport.EmployeeWithContract(IDEmployee, oState, xShiftDate) Then
                    ' Dim xFreezingDate = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, False, oState, oTrans.Connection)

                    If Not roBusinessSupport.InPeriodOfFreezing(xShiftDate, IDEmployee, oState) Then
                        Try

                            Dim bolAssign As Boolean = True

                            Dim strSQL As String
                            strSQL = "@SELECT# * FROM DailySchedule WHERE"
                            strSQL &= " DailySchedule.IDEmployee = " & IDEmployee & " AND"
                            strSQL &= " DailySchedule.Date = " & Any2Time(xShiftDate).SQLSmallDateTime

                            Dim tb As New DataTable
                            Dim cmd As DbCommand = CreateCommand(strSQL)
                            Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)
                            ad.Fill(tb)

                            Dim oRow As DataRow

                            If tb.Rows.Count = 0 Then
                                oRow = tb.NewRow
                                oRow("IDEmployee") = IDEmployee
                                oRow("Date") = xShiftDate
                            Else
                                oRow = tb.Rows(0)
                            End If

                            oRow("LockedDay") = xLock

                            ad.Update(tb)

                            bolRet = (oState.Result = EmployeeResultEnum.NoError)
                        Catch ex As DbException
                            oState.UpdateStateInfo(ex, "roEmployees::LockDay")
                        Catch ex As Exception
                            oState.UpdateStateInfo(ex, "roEmployees::LockDay")
                        End Try
                    Else
                        oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                    End If
                Else
                    oState.Result = EmployeeResultEnum.EmployeeNoActiveContract
                End If
            Else
                oState.Result = EmployeeResultEnum.AccessDenied
            End If

            Return bolRet

        End Function

        Public Shared Function MassCausePartialPeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer, ByRef oState As Employee.roEmployeeState, ByVal xFirstDate As Date, ByVal bolCheckPermission As Boolean, ByVal strFeaturePermission As String,
                                                Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim sSQL As String = ""
            Dim massDay As Boolean = False

            Dim xFreezingDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, False, oState)
            'If xFirstDate > xFreezingDateEmployee Then xFreezingDateEmployee = xFirstDate

            Dim oShiftState As New Shift.roShiftState()
            oShiftState.IDPassport = oState.IDPassport

            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport, LoadType.Passport)

            Dim bolHasPermission As Boolean = True

            Dim intIDUser As Integer = 0
            Try
                If oPassport.IDUser.HasValue Then intIDUser = oPassport.IDUser
            Catch ex As Exception
            End Try

            Dim xDate As Date = xBeginPeriod

            bolRet = True

            sSQL = "@SELECT# * FROM Punches WHERE IDEmployee = " & IDEmployee.ToString
            sSQL = sSQL & " AND ActualType IN(1,2)   "
            sSQL = sSQL & " AND DateTime BETWEEN " & Any2Time(xBeginPeriod).SQLDateTime & " AND " & Any2Time(xEndPeriod).SQLDateTime

            Dim oPunch As Punch.roPunch
            Dim oPunchState As Punch.roPunchState
            Dim tb As New DataTable()
            Dim cmd As DbCommand = CreateCommand(sSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tb)

            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    'Solamente justifica fichajes si no estan en periodo de congelacion
                    If CDate(oRow("ShiftDate")) > CDate(xFreezingDateEmployee) Then
                        If bolCheckPermission Then
                            ' Verificamos los permisos del pasaporte actual sobre la feature concreta
                            bolHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, strFeaturePermission, Permission.Write, IDEmployee, CDate(oRow("ShiftDate")), )
                        End If

                        If bolHasPermission Then
                            'Cargo fichaje
                            oPunchState = New Punch.roPunchState(oState.IDPassport)
                            oPunch = New Punch.roPunch(oRow("IDEmployee"), oRow("ID"), oPunchState)
                            oPunch.TypeData = intIDCause
                            oPunch.Action = 2
                            oPunch.Source = PunchSource.StandardImport
                            oPunch.Save(, True)

                            ' Asignamos justificacion y usuario que ha modificado
                            'oRow("TypeData") = intIDCause
                            'oRow("IDPassport") = oState.IDPassport
                            'oRow("Action") = 2

                            'Marcamos ese dia para recalcular
                            sSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 0, [GUID] = '' WHERE Date = " & Any2Time(CDate(oRow("ShiftDate"))).SQLSmallDateTime & " AND IDEmployee = " & IDEmployee.ToString
                            ExecuteScalar(sSQL)
                        Else
                            oState.Result = EmployeeResultEnum.AccessDenied
                        End If
                    Else
                        oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                    End If
                Next

                'da.Update(tb)
            End If

            Return bolRet

        End Function

        Public Shared Function MassCauseCompletePeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer, ByRef oState As Employee.roEmployeeState, ByVal xFirstDate As Date, ByVal bolCheckPermission As Boolean, ByVal strFeaturePermission As String,
                                                Optional ByVal bAudit As Boolean = False)
            Dim bolRet As Boolean = False

            Dim sSQL As String = ""
            Dim massDay As Boolean = False

            Dim xFreezingDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, False, oState)
            'If xFirstDate > xFreezingDateEmployee Then xFreezingDateEmployee = xFirstDate

            Dim oShiftState As New Shift.roShiftState()
            oShiftState.IDPassport = oState.IDPassport

            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport, LoadType.Passport)

            Dim bolHasPermission As Boolean = True

            Dim intIDUser As Integer = 0
            Try
                If oPassport.IDUser.HasValue Then intIDUser = oPassport.IDUser
            Catch ex As Exception
            End Try

            Dim xDate As Date = xBeginPeriod

            bolRet = True

            While xDate <= xEndPeriod
                ' Solo justificamos si la fecha  es posterior a la fecha de congelacion
                If xDate > CDate(xFreezingDateEmployee) Then
                    Try
                        If bolCheckPermission Then
                            ' Verificamos los permisos del pasaporte actual sobre la feature concreta
                            bolHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, strFeaturePermission, Permission.Write, IDEmployee, xDate, )
                        End If

                        ' Si tiene permisos
                        If bolHasPermission Then
                            'Solamente justifica incidencias si el empleado tiene contrato
                            If roBusinessSupport.EmployeeWithContract(IDEmployee, oState, xDate) Then
                                'Comprobamos que no haya ninguna incidencia de trabajo
                                sSQL = "@SELECT# ISNULL(COUNT(*),0) FROM DailyIncidences WHERE IDType IN (@SELECT# ID FROM sysroDailyIncidencesTypes WHERE WorkingTime = 1) AND Date = " & Any2Time(xDate).SQLSmallDateTime & " AND IDEmployee = " & IDEmployee
                                massDay = IIf(Any2Double(ExecuteScalar(sSQL)) = 0, True, False)

                                If massDay Then
                                    'Borramos las justificaciones diarias para ese dia de la DailyCause, que no vengan de reglas
                                    sSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & IDEmployee & " AND Date = " & Any2Time(xDate).SQLSmallDateTime & " AND AccrualsRules=0 AND IDRelatedIncidence > 0 "
                                    ExecuteScalar(sSQL)

                                    'Generamos datos en la DailyCauses para cada registro de Dailyincidences
                                    sSQL = "@SELECT# * FROM DailyIncidences WHERE Date = " & Any2Time(xDate).SQLSmallDateTime & " AND IDEmployee = " & IDEmployee
                                    Dim tb As DataTable = CreateDataTable(sSQL, )
                                    If tb IsNot Nothing Then

                                        For Each oRow As DataRow In tb.Rows
                                            'Generamos la inserción
                                            sSQL = "@INSERT# INTO DailyCauses (IDEmployee, Date, IDRelatedIncidence, IDCause, Value, Manual,CauseUser,CauseUserType, AccrualsRules, IsNotReliable) VALUES"
                                            sSQL = sSQL & " (" & IDEmployee & ", " & Any2Time(xDate).SQLSmallDateTime & ", " & Any2Double(oRow("ID")) & ", " & intIDCause.ToString & ", " & Replace(Any2Double(oRow("Value")), ",", ".") & ",1," & intIDUser.ToString & ",0,0,0)"
                                            ExecuteScalar(sSQL)

                                            'Marcamos ese dia para recalcular
                                            sSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' WHERE Date = " & Any2Time(xDate).SQLSmallDateTime & " AND IDEmployee = " & IDEmployee.ToString
                                            ExecuteScalar(sSQL)
                                        Next
                                    End If
                                End If
                            End If
                        Else
                            oState.Result = EmployeeResultEnum.AccessDenied
                        End If
                    Catch ex As Exception
                        bolRet = False
                        oState.UpdateStateInfo(ex, "roEmployees::MassCauseCompletePeriod")
                    End Try
                Else
                    oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                End If

                xDate = xDate.AddDays(1)
            End While

            Return bolRet
        End Function

        Public Shared Function AssignShiftData(ByVal IDEmployee As Integer, ByVal xShiftDate As Date, ByVal _LayersDefinition As String, ByVal _ExpectedWorkingHours As Double,
                                               ByRef _LockedDayAction As LockedDayAction, ByRef oState As Employee.roEmployeeState)
            Dim bolRet As Boolean = False

            Dim oShiftState As New Shift.roShiftState()
            oShiftState.IDPassport = oState.IDPassport

            Dim bolAssign As Boolean = False
            Dim strSQL As String
            strSQL = "@SELECT# * FROM DailySchedule WHERE"
            strSQL &= " DailySchedule.IDEmployee = " & IDEmployee & " AND"
            strSQL &= " DailySchedule.Date = " & Any2Time(xShiftDate).SQLSmallDateTime

            Dim tb As New DataTable
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)
            ad.Fill(tb)

            Dim oRow As DataRow

            If tb.Rows.Count = 0 Then
            Else
                oRow = tb.Rows(0)
                If Any2Boolean(oRow("LockedDay")) Then
                    ' El día a asignar está bloqueado.
                    Select Case _LockedDayAction
                        Case LockedDayAction.ReplaceFirst, LockedDayAction.ReplaceAll
                            bolAssign = True
                    End Select
                Else
                    bolAssign = True
                End If

                If bolAssign Then
                    'Asignamos los datos complementarios y flotantes
                    oRow("ExpectedWorkingHours") = IIf(_ExpectedWorkingHours = -1, DBNull.Value, _ExpectedWorkingHours)
                    oRow("LayersDefinition") = IIf(_LayersDefinition = "", DBNull.Value, _LayersDefinition)
                    ad.Update(tb)
                End If
            End If

            Return bolRet

        End Function

        Public Shared Function AssignShift(ByVal IDEmployee As Integer, ByVal xShiftDate As Date, ByVal intIDShift1 As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer, ByVal intIDShift4 As Integer,
                            ByVal _StartShift1 As DateTime,
                            ByVal _StartShift2 As DateTime,
                            ByVal _StartShift3 As DateTime,
                            ByVal _StartShift4 As DateTime,
                            ByVal intIDAssignment As Integer,
                            ByRef _LockedDayAction As LockedDayAction, ByRef _CoverageDayAction As LockedDayAction, ByRef oState As Employee.roEmployeeState,
                            Optional ByVal bolCheckPermission As Boolean = True, Optional ByVal bolNotify As Boolean = True,
                            Optional ByVal bAudit As Boolean = False,
                            Optional ByVal _ShiftPermissionAction As ShiftPermissionAction = ShiftPermissionAction.ContinueAll,
                            Optional ByVal intIDShiftBase As Integer = -2, Optional ByVal intIDAssignmentBase As Integer = -2,
                            Optional ByVal xStartDateBase As DateTime = Nothing, Optional ByVal isHolidaysBase As Boolean = False,
                            Optional ByVal sRequestFeaturePermission As String = "", Optional bKeepBudget As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False

            Dim oShiftState As New Shift.roShiftState()
            oShiftState.IDPassport = oState.IDPassport

            Dim bContinuar As Boolean = True
            If Not Shift.roShift.ShiftIsAllowed(oShiftState, intIDShift1, intIDShift2, intIDShift3, intIDShift4) Then

                If _ShiftPermissionAction = ShiftPermissionAction.None Or _ShiftPermissionAction = ShiftPermissionAction.StopAll Then
                    oState.Result = EmployeeResultEnum.ShiftWithoutPermission
                ElseIf _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Or _ShiftPermissionAction = ShiftPermissionAction.ContinueAll Then
                    oState.Result = EmployeeResultEnum.NoError
                End If

                bContinuar = False

            End If

            If bContinuar Then
                Dim bolHasPermission As Boolean = True

                If bolCheckPermission Then
                    ' Verificamos los permisos del pasaporte actual sobre la planificación
                    bolHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, IDEmployee, xShiftDate, )
                    ' Si viene de una petición de vacaciones y tiene permisos para aceptarla y no para calendario, se permite que el horario se valide igualmente
                    If sRequestFeaturePermission <> String.Empty AndAlso bolHasPermission = False Then '"Calendar.Requests.Vacations"
                        bolHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, sRequestFeaturePermission, Permission.Write, IDEmployee, xShiftDate, )
                    End If
                End If

                If bolHasPermission Then
                    'Solamente asigna el horario si este tiene contrato
                    If roBusinessSupport.EmployeeWithContract(IDEmployee, oState, xShiftDate) Then
                        If Not roBusinessSupport.InPeriodOfFreezing(xShiftDate, IDEmployee, oState) Then
                            Try
                                Dim bolAssign As Boolean = True

                                Dim strSQL As String
                                strSQL = "@SELECT# * FROM DailySchedule WHERE"
                                strSQL &= " DailySchedule.IDEmployee = " & IDEmployee & " AND"
                                strSQL &= " DailySchedule.Date = " & Any2Time(xShiftDate).SQLSmallDateTime

                                Dim tb As New DataTable
                                Dim cmd As DbCommand = CreateCommand(strSQL)
                                Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)

                                ad.Fill(tb)

                                Dim bolModifiedShift As Boolean = False

                                Dim oRow As DataRow

                                If tb.Rows.Count = 0 Then
                                    oRow = tb.NewRow
                                    bolIsNew = True
                                    oRow("IDEmployee") = IDEmployee
                                    oRow("Date") = xShiftDate
                                    oRow("LockedDay") = False
                                Else
                                    oRow = tb.Rows(0)
                                    If Any2Boolean(oRow("LockedDay")) Then
                                        ' El día a asignar está bloqueado.
                                        Select Case _LockedDayAction
                                            Case LockedDayAction.None
                                                ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                oState.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                                bolAssign = False
                                            Case LockedDayAction.ReplaceFirst
                                                ' Se tiene que asignar igualmente
                                                bolAssign = True
                                                _LockedDayAction = LockedDayAction.None
                                            Case LockedDayAction.ReplaceAll
                                                ' Se tiene que asignar igualmente
                                                bolAssign = True
                                            Case LockedDayAction.NoReplaceFirst
                                                ' No se tiene que asignar
                                                bolAssign = False
                                                _LockedDayAction = LockedDayAction.None
                                            Case LockedDayAction.NoReplaceAll
                                                ' No se tiene que asignar
                                                bolAssign = False
                                        End Select
                                    End If

                                    If bKeepBudget Then
                                        ' Si se tiene que mantener el presupuesto y esta asignado a uno, no podemos modificar la planificacion
                                        If Any2Long(oRow("IDDailyBudgetPosition")) > 0 Then
                                            ' No se tiene que asignar
                                            bolAssign = False
                                        End If
                                    End If
                                End If

                                Dim bolRemoveholidays As Boolean = False
                                If intIDShift1 <= 0 Then
                                    ' Queremos quitar el horario de vacaciones
                                    bolRemoveholidays = True
                                End If

                                ' Verificamos que los horarios a asignar sean correctos (no haya duplicados)
                                If bolAssign Then
                                    Dim _IDShift1 As Integer = 0
                                    If intIDShift1 > 0 Then
                                        _IDShift1 = intIDShift1
                                    ElseIf intIDShift1 = 0 Then
                                        _IDShift1 = Any2Integer(oRow("IDShift1"))
                                        If Not IsDBNull(oRow("StartShift1")) Then
                                            _StartShift1 = oRow("StartShift1")
                                        Else
                                            _StartShift1 = Nothing
                                        End If
                                    End If
                                    Dim _IDShift2 As Integer = 0
                                    If intIDShift2 > 0 Then
                                        _IDShift2 = intIDShift2
                                    ElseIf intIDShift2 = 0 Then
                                        _IDShift2 = Any2Integer(oRow("IDShift2"))
                                        If Not IsDBNull(oRow("StartShift2")) Then
                                            _StartShift2 = oRow("StartShift2")
                                        Else
                                            _StartShift2 = Nothing
                                        End If
                                    End If
                                    Dim _IDShift3 As Integer = 0
                                    If intIDShift3 > 0 Then
                                        _IDShift3 = intIDShift3
                                    ElseIf intIDShift3 = 0 Then
                                        _IDShift3 = Any2Integer(oRow("IDShift3"))
                                        If Not IsDBNull(oRow("StartShift3")) Then
                                            _StartShift3 = oRow("StartShift3")
                                        Else
                                            _StartShift3 = Nothing
                                        End If
                                    End If
                                    Dim _IDShift4 As Integer = 0
                                    If intIDShift4 > 0 Then
                                        _IDShift4 = intIDShift4
                                    ElseIf intIDShift4 = 0 Then
                                        _IDShift4 = Any2Integer(oRow("IDShift4"))
                                        If Not IsDBNull(oRow("StartShift4")) Then
                                            _StartShift4 = oRow("StartShift4")
                                        Else
                                            _StartShift4 = Nothing
                                        End If
                                    End If

                                    Dim bCompact As Boolean = False
                                    ' Miramos que los horarios planificados no estén ya planificados para ese empleado y día, pero en otra posición ... (alternativos)
                                    If _IDShift1 > 0 Then
                                        If _IDShift1 = _IDShift2 AndAlso _StartShift1 = _StartShift2 Then
                                            ' Eliminio el horario 2
                                            intIDShift2 = -1
                                            _IDShift2 = 0
                                            _StartShift2 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift1 = _IDShift3 AndAlso _StartShift1 = _StartShift3 Then
                                            ' Eliminio el horario 3
                                            intIDShift3 = -1
                                            _IDShift3 = 0
                                            _StartShift3 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift1 = _IDShift4 AndAlso _StartShift1 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                    End If
                                    If bolAssign And _IDShift2 > 0 Then 'And ((_IDShift2 = _IDShift3 And _StartShift2 = _StartShift3) Or (_IDShift2 = _IDShift4 And _StartShift2 = _StartShift4)) Then
                                        If _IDShift2 = _IDShift3 And _StartShift2 = _StartShift3 Then
                                            ' Eliminio el horario 3
                                            intIDShift3 = -1
                                            _IDShift3 = 0
                                            _StartShift3 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift2 = _IDShift4 And _StartShift2 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                    End If
                                    If bolAssign And _IDShift3 > 0 Then 'And ((_IDShift3 = _IDShift2 And _StartShift3 = _StartShift2) Or (_IDShift3 = _IDShift4 And _StartShift3 = _StartShift4)) Then
                                        If _IDShift3 = _IDShift4 And _StartShift3 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                    End If
                                    'TODO: Compactamos si es necesario (se borró algún alternativo
                                    If Not bolAssign Then oState.Result = EmployeeResultEnum.ShiftAlreadyAssigned

                                End If

                                Dim baseShiftId As Integer = -1
                                Dim baseStartDate As Date = Nothing
                                Dim dBaseStartFlexibleDate As Date = Nothing
                                Dim dBaseEndFlexibleDate As Date = Nothing
                                Dim iBaseFlexibleColor As Integer = Nothing
                                Dim sBaseFlexibleName As String = Nothing
                                Dim dStartFlexibleDate As Date = Nothing
                                Dim dEndFlexibleDate As Date = Nothing
                                Dim iFlexibleColor As Integer = Nothing
                                Dim sFlexibleName As String = Nothing
                                Dim isHolidays As Boolean = False
                                Dim baseAssignmentId As Integer = -1

                                'Si me pasan parametros para el turno base, es que vamos a copiarlo
                                If (intIDAssignmentBase = -2 AndAlso intIDShiftBase = -2) Then
                                    'No se permite asignar un horario de vacaciones a un dia sin planificar
                                    If bolAssign AndAlso intIDShift1 > 0 AndAlso Shift.roShift.IsHolidays(intIDShift1) AndAlso Any2Integer(oRow("IDShift1")) = 0 Then
                                        bolAssign = False

                                        If Not bolAssign Then oState.Result = EmployeeResultEnum.NoBaseShiftAssigned
                                    End If

                                    'Miramos si el horario que tiene asignado actualmente ya es de vacaciones
                                    Dim actuallyHolidays As Boolean = roTypes.Any2Boolean(oRow("IsHolidays"))
                                    Dim idHolidayShift As Integer = 0
                                    If actuallyHolidays Then
                                        idHolidayShift = Any2Integer(oRow("IDShift1"))
                                    End If

                                    'Miramos si el horario que nos quieren asignar es de vacaciones
                                    Dim intHolidayShift As Integer = 0
                                    If (bolAssign AndAlso Shift.roShift.IsHolidays(intIDShift1)) Then

                                        Dim vacShift As Shift.roShift = New Shift.roShift(intIDShift1, oShiftState, False)
                                        If vacShift.AreWorkingDays Then
                                            Dim baseShift As Shift.roShift = Nothing

                                            If actuallyHolidays Then
                                                baseShift = New Shift.roShift(Any2Integer(oRow("IDShiftBase")), oShiftState, False)
                                            Else
                                                baseShift = New Shift.roShift(Any2Integer(oRow("IDShift1")), oShiftState, False)
                                            End If

                                            'Si el horario base no tiene horas y el horario de vacaciones es solo para laborables no dejamos asignar
                                            If baseShift.ExpectedWorkingHours = 0 Then
                                                bolAssign = False
                                                oState.Result = EmployeeResultEnum.NoWorkingDay
                                            End If
                                        End If

                                        intHolidayShift = intIDShift1
                                        intIDShift1 = 0
                                    End If

                                    If bolAssign Then
                                        If intHolidayShift > 0 Then 'Si nos quieren asignar un horario de vacaciones
                                            'Si el empleado no tiene planificación no copiamos la planificacion
                                            If actuallyHolidays Then 'Si ya estamos en vacaciones guardamos el shift 1 y mantenemos el base ya existente
                                                intIDShift1 = intHolidayShift
                                                baseShiftId = Any2Integer(oRow("IDShiftBase"))

                                                If Not IsDBNull(oRow("StartShiftBase")) Then
                                                    baseStartDate = oRow("StartShiftBase")
                                                Else
                                                    baseStartDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                    dBaseStartFlexibleDate = oRow("StartFlexibleBase")
                                                Else
                                                    dBaseStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                    dBaseEndFlexibleDate = oRow("EndFlexibleBase")
                                                Else
                                                    dBaseEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                    sBaseFlexibleName = oRow("ShiftNameBase")
                                                Else
                                                    sBaseFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                    iBaseFlexibleColor = oRow("ShiftColorBase")
                                                Else
                                                    iBaseFlexibleColor = Nothing
                                                End If

                                                baseAssignmentId = Any2Integer(oRow("IDAssignmentBase"))

                                                If idHolidayShift <> intIDShift1 Then
                                                    oRow("TimestampHolidays") = Date.Now
                                                End If
                                            Else 'Si no estoy en vacaciones pasamos los datos actuales al base y guardamos el shift1
                                                baseShiftId = Any2Integer(oRow("IDShift1"))

                                                If Not IsDBNull(oRow("StartShift1")) Then
                                                    baseStartDate = oRow("StartShift1")
                                                Else
                                                    baseStartDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("StartFlexible1")) Then
                                                    dBaseStartFlexibleDate = oRow("StartFlexible1")
                                                Else
                                                    dBaseStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexible1")) Then
                                                    dBaseEndFlexibleDate = oRow("EndFlexible1")
                                                Else
                                                    dBaseEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftName1")) Then
                                                    sBaseFlexibleName = oRow("ShiftName1")
                                                Else
                                                    sBaseFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColor1")) Then
                                                    iBaseFlexibleColor = oRow("ShiftColor1")
                                                Else
                                                    iBaseFlexibleColor = Nothing
                                                End If

                                                baseAssignmentId = Any2Integer(oRow("IDAssignment"))
                                                intIDShift1 = intHolidayShift

                                                oRow("TimestampHolidays") = Date.Now
                                            End If
                                            isHolidays = True
                                        Else 'Si asignamos un horario que no es de vacaciones

                                            Dim Aux_intIDShift1 As Integer = intIDShift1
                                            Dim Aux__StartShift1 As Date = _StartShift1
                                            Dim Aux_StartFlexible1 As Date = dStartFlexibleDate
                                            Dim Aux_EndFlexible1 As Date = dEndFlexibleDate
                                            Dim Aux_ColorFlexible1 As Integer = iFlexibleColor
                                            Dim Aux_FlexibleShiftName1 As String = sFlexibleName

                                            If intIDShift1 <= 0 Then 'Si pasamos un idshift1 como 0, significa que vamos a quitar el horario de vacaciones. Pasamos la info del base al shift1
                                                intIDShift1 = Any2Integer(oRow("IDShiftBase"))
                                                If Not IsDBNull(oRow("StartShiftBase")) Then
                                                    _StartShift1 = oRow("StartShiftBase")
                                                Else
                                                    _StartShift1 = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                    dStartFlexibleDate = oRow("StartFlexibleBase")
                                                Else
                                                    dStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                    dEndFlexibleDate = oRow("EndFlexibleBase")
                                                Else
                                                    dEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                    sFlexibleName = oRow("ShiftNameBase")
                                                Else
                                                    sFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                    iFlexibleColor = oRow("ShiftColorBase")
                                                Else
                                                    iFlexibleColor = Nothing
                                                End If

                                                intIDAssignment = Any2Integer(oRow("IDAssignmentBase"))

                                                'Registramos que se ha eliminado un dia que estaba con vacaciones
                                                Dim oProgrammedManager As New roProgrammedHolidayManager
                                                Dim dNow As DateTime = Date.Now
                                                oProgrammedManager.RegisterDeleteProgrammedHoliday(IDEmployee, , xShiftDate, dNow)
                                                oRow("TimestampHolidays") = dNow

                                            End If

                                            ' Si asignamos un horario normal a un dia que tiene vacaciones ya asignadas
                                            ' debemos reemplazar el horario base por el que nos pasan
                                            If Aux_intIDShift1 > 0 AndAlso actuallyHolidays Then
                                                isHolidays = True
                                                baseShiftId = Aux_intIDShift1
                                                baseStartDate = Aux__StartShift1
                                                dBaseStartFlexibleDate = Aux_StartFlexible1
                                                dBaseEndFlexibleDate = Aux_EndFlexible1
                                                iBaseFlexibleColor = Aux_ColorFlexible1
                                                sBaseFlexibleName = Aux_FlexibleShiftName1

                                                intIDShift1 = Any2Integer(oRow("IDShift1"))
                                                If Not IsDBNull(oRow("StartShift1")) Then
                                                    _StartShift1 = oRow("StartShift1")
                                                Else
                                                    _StartShift1 = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexible1")) Then
                                                    dStartFlexibleDate = oRow("StartFlexible1")
                                                Else
                                                    dStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexible1")) Then
                                                    dEndFlexibleDate = oRow("EndFlexible1")
                                                Else
                                                    dEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftName1")) Then
                                                    sFlexibleName = oRow("ShiftName1")
                                                Else
                                                    sFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColor1")) Then
                                                    iFlexibleColor = oRow("ShiftColor1")
                                                Else
                                                    iFlexibleColor = Nothing
                                                End If
                                            Else
                                                'Al asignar un horario borramos la información que contiene el base
                                                baseShiftId = -1
                                                baseStartDate = Nothing
                                                baseAssignmentId = -1

                                                dBaseStartFlexibleDate = Nothing
                                                dBaseEndFlexibleDate = Nothing
                                                iBaseFlexibleColor = 0
                                                sBaseFlexibleName = Nothing

                                                isHolidays = False
                                            End If

                                        End If
                                    End If
                                Else
                                    baseShiftId = intIDShiftBase
                                    baseStartDate = xStartDateBase
                                    isHolidays = isHolidaysBase
                                    baseAssignmentId = intIDAssignmentBase
                                End If

                                Dim bolAssignmentChanged As Boolean = False

                                If Not isHolidays Then 'Si no estamos en vacaciones comprobamos la idoniedad del puesto si procede
                                    ' Verificamos que el puesto sea compatible con el empleado y el horario
                                    If bolAssign Then

                                        Dim _IDAssignment As Integer = 0
                                        If intIDAssignment > 0 Then
                                            _IDAssignment = intIDAssignment
                                        ElseIf intIDAssignment = 0 Then
                                            _IDAssignment = Any2Integer(oRow("IDAssignment"))
                                        End If

                                        Dim _IDShift1 As Integer = 0
                                        If intIDShift1 > 0 Then
                                            _IDShift1 = intIDShift1
                                        ElseIf intIDShift1 = 0 Then
                                            _IDShift1 = Any2Integer(oRow("IDShift1"))
                                        End If

                                        If _IDShift1 > 0 And _IDAssignment > 0 Then

                                            ' Si el empleado no tiene este puesto asignado en su definición, se borra el puesto de la DailySchedule
                                            If Not roBusinessSupport.ExistEmployeeAssignment(IDEmployee, _IDAssignment, oState) Then
                                                intIDAssignment = -1
                                            End If

                                            ' Si el horario principal no tiene este puesto asignado, se borra el puesto
                                            roBusinessState.CopyTo(oState, oShiftState)
                                            If Not Shift.roShiftAssignment.ExistShiftAssignment(_IDShift1, _IDAssignment, oShiftState) Then
                                                intIDAssignment = -1
                                            End If
                                            roBusinessState.CopyTo(oShiftState, oState)
                                        Else
                                            intIDAssignment = -1
                                        End If

                                    End If

                                    If bolAssign Then

                                        ' Si se está modificando el puesto, verificamos que no haya cobertura
                                        If intIDAssignment > 0 Then
                                            bolAssignmentChanged = (Any2Integer(oRow("IDAssignment")) <> intIDAssignment)
                                        ElseIf intIDAssignment = -1 Then
                                            bolAssignmentChanged = (Not IsDBNull(oRow("IDAssignment")))
                                        End If

                                        If bolAssignmentChanged AndAlso (Any2Boolean(oRow("IsCovered")) OrElse Any2Integer(oRow("IDEmployeeCovered")) > 0) Then

                                            Select Case _CoverageDayAction
                                                Case LockedDayAction.None
                                                    ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                    oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay
                                                    bolAssign = False
                                                Case LockedDayAction.ReplaceFirst
                                                    ' Se tiene que asignar igualmente
                                                    bolAssign = True
                                                    _CoverageDayAction = LockedDayAction.None
                                                Case LockedDayAction.ReplaceAll
                                                    ' Se tiene que asignar igualmente
                                                    bolAssign = True
                                                Case LockedDayAction.NoReplaceFirst
                                                    ' No se tiene que asignar
                                                    bolAssign = False
                                                    _CoverageDayAction = LockedDayAction.None
                                                Case LockedDayAction.NoReplaceAll
                                                    ' No se tiene que asignar
                                                    bolAssign = False
                                            End Select

                                        End If

                                    End If
                                Else
                                    'Si estoy de vacaciones pongo el idassignment a 0, anteriormente ya lo he guardado en el base si procede
                                    intIDAssignment = -1
                                End If

                                If bolAssign Then
                                    If intIDShift1 > 0 Then
                                        Dim intOldShift As Integer = roTypes.Any2Integer(oRow("IDShift1"))
                                        If IsDBNull(oRow("IDShift1")) OrElse roTypes.Any2Double(oRow("IDShift1")) = 0 Then
                                            bolModifiedShift = False
                                        Else
                                            ' Si previamente tiene asignado un horario,
                                            ' en caso necesario hay que revisar el nuevo horario asignado
                                            ' para la notificacion de horario asignado
                                            bolModifiedShift = True
                                            If intOldShift <> intIDShift1 Then
                                                oRow("IDPreviousShift") = intOldShift
                                            End If
                                        End If

                                        oRow("IDShift1") = intIDShift1
                                        If _StartShift1 <> Nothing Then
                                            oRow("StartShift1") = _StartShift1
                                        Else
                                            oRow("StartShift1") = DBNull.Value
                                        End If

                                        ' Starter con horario flexible
                                        If dStartFlexibleDate <> Nothing Then
                                            oRow("StartFlexible1") = dStartFlexibleDate
                                        Else
                                            oRow("StartFlexible1") = DBNull.Value
                                        End If

                                        If dEndFlexibleDate <> Nothing Then
                                            oRow("EndFlexible1") = dEndFlexibleDate
                                        Else
                                            oRow("EndFlexible1") = DBNull.Value
                                        End If

                                        If iFlexibleColor <> Nothing AndAlso iFlexibleColor > 0 Then
                                            oRow("ShiftColor1") = iFlexibleColor
                                        Else
                                            oRow("ShiftColor1") = DBNull.Value
                                        End If

                                        If sFlexibleName <> Nothing AndAlso sFlexibleName <> String.Empty Then
                                            oRow("ShiftName1") = sFlexibleName
                                        Else
                                            oRow("ShiftName1") = DBNull.Value
                                        End If

                                        ' Verificamos si hay que revisar la notificacion de Asignacion de horario
                                        Dim oNotificationState As New Notifications.roNotificationState(-1)
                                        Dim oAssignedShiftNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 51 And Activated=1", oNotificationState,, True)

                                        If oAssignedShiftNotifications IsNot Nothing AndAlso oAssignedShiftNotifications.Count > 0 AndAlso bolModifiedShift AndAlso xShiftDate > DateTime.Now.Date Then
                                            Notifications.roNotification.ExecuteAssignedShiftNotification(oNotificationState, oAssignedShiftNotifications, IDEmployee, xShiftDate, intIDShift1, intOldShift, False)
                                        End If

                                    ElseIf intIDShift1 = -1 Then
                                        oRow("IDShift1") = DBNull.Value
                                        oRow("IDPreviousShift") = DBNull.Value
                                        oRow("StartShift1") = DBNull.Value

                                        oRow("StartFlexible1") = DBNull.Value
                                        oRow("EndFlexible1") = DBNull.Value
                                        oRow("ShiftColor1") = DBNull.Value
                                        oRow("Shiftname1") = DBNull.Value
                                    End If
                                    If intIDShift2 > 0 Then
                                        oRow("IDShift2") = intIDShift2
                                        If _StartShift2 <> Nothing Then
                                            oRow("StartShift2") = _StartShift2
                                        Else
                                            oRow("StartShift2") = DBNull.Value
                                        End If
                                    ElseIf intIDShift2 = -1 Then
                                        oRow("IDShift2") = DBNull.Value
                                        oRow("StartShift2") = DBNull.Value
                                    End If
                                    If intIDShift3 > 0 Then
                                        oRow("IDShift3") = intIDShift3
                                        If _StartShift3 <> Nothing Then
                                            oRow("StartShift3") = _StartShift3
                                        Else
                                            oRow("StartShift3") = DBNull.Value
                                        End If
                                    ElseIf intIDShift3 = -1 Then
                                        oRow("IDShift3") = DBNull.Value
                                        oRow("StartShift3") = DBNull.Value
                                    End If
                                    If intIDShift4 > 0 Then
                                        oRow("IDShift4") = intIDShift4
                                        If _StartShift4 <> Nothing Then
                                            oRow("StartShift4") = _StartShift4
                                        Else
                                            oRow("StartShift4") = DBNull.Value
                                        End If
                                    ElseIf intIDShift4 = -1 Then
                                        oRow("IDShift4") = DBNull.Value
                                        oRow("StartShift4") = DBNull.Value
                                    End If

                                    If intIDAssignment > 0 Then
                                        oRow("IDAssignment") = intIDAssignment
                                    ElseIf intIDAssignment = -1 Then
                                        oRow("IDAssignment") = DBNull.Value
                                    End If

                                    If baseShiftId > 0 Then
                                        oRow("IDShiftBase") = baseShiftId
                                        If baseStartDate <> Nothing Then
                                            oRow("StartShiftBase") = baseStartDate
                                        Else
                                            oRow("StartShiftBase") = DBNull.Value
                                        End If

                                        If dBaseStartFlexibleDate <> Nothing Then
                                            oRow("StartFlexibleBase") = dBaseStartFlexibleDate
                                        Else
                                            oRow("StartFlexibleBase") = DBNull.Value
                                        End If
                                        If dBaseEndFlexibleDate <> Nothing Then
                                            oRow("EndFlexibleBase") = dBaseEndFlexibleDate
                                        Else
                                            oRow("EndFlexibleBase") = DBNull.Value
                                        End If
                                        If iBaseFlexibleColor <> Nothing AndAlso iBaseFlexibleColor > 0 Then
                                            oRow("ShiftColorBase") = iBaseFlexibleColor
                                        Else
                                            oRow("ShiftColorBase") = DBNull.Value
                                        End If
                                        If sBaseFlexibleName <> Nothing AndAlso sBaseFlexibleName <> String.Empty Then
                                            oRow("ShiftNameBase") = sBaseFlexibleName
                                        Else
                                            oRow("ShiftNameBase") = DBNull.Value
                                        End If

                                    ElseIf baseShiftId = -1 Then
                                        oRow("IDShiftBase") = DBNull.Value
                                        oRow("StartShiftBase") = DBNull.Value

                                        oRow("StartFlexibleBase") = DBNull.Value
                                        oRow("EndFlexibleBase") = DBNull.Value
                                        oRow("ShiftColorBase") = DBNull.Value
                                        oRow("ShiftNameBase") = DBNull.Value
                                    End If

                                    If baseAssignmentId > 0 Then
                                        oRow("IDAssignmentBase") = baseAssignmentId
                                    ElseIf baseAssignmentId = -1 Then
                                        oRow("IDAssignmentBase") = DBNull.Value
                                    End If

                                    If isHolidays Then
                                        oRow("IsHolidays") = isHolidays
                                    Else
                                        oRow("IsHolidays") = DBNull.Value
                                    End If

                                    oRow("IDDailyBudgetPosition") = DBNull.Value

                                    If bolAssignmentChanged Then
                                        ' Si se está modificando el puesto, borramos la posible cobertura
                                        If Any2Boolean(oRow("IsCovered")) Then

                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                             "SET IDAssignment = OldIDAssignment, " &
                                                 "OldIDAssignment = NULL, " &
                                                 "IDEmployeeCovered = NULL, " &
                                                 "Status = 0, [GUID] = '' " &
                                             "WHERE IDEmployeeCovered = " & IDEmployee.ToString & " AND " &
                                                   "Date = " & Any2Time(xShiftDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)

                                        ElseIf Any2Integer(oRow("IDEmployeeCovered")) > 0 Then

                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                             "SET IsCovered = 0, " &
                                                 "Status = 0, [GUID] = '' " &
                                             "WHERE IDEmployee = " & Any2Integer(oRow("IDEmployeeCovered")) & " AND " &
                                                   "Date = " & Any2Time(xShiftDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)

                                        End If
                                        oRow("IsCovered") = DBNull.Value
                                        oRow("OldIDAssignment") = DBNull.Value
                                        oRow("IDEmployeeCovered") = DBNull.Value
                                    End If

                                    ' Si planificamos un horario de vacaciones, dejamos lo que haya
                                    ' Si planificamos un horario diferente al de vacaciones siempre lo eliminamos salvo
                                    ' en el caso que indiquemos que queremos quitar las vacaciones que en ese caso los dejamos
                                    ' Si el horario base no tiene horas teoricas , tambien eliminamos los datos
                                    Dim bolBaseShiftwithExpected As Boolean = False
                                    If baseShiftId <> -1 Then
                                        Dim baseShift = New Shift.roShift(baseShiftId, oShiftState, False)
                                        If baseShift.AllowComplementary Or baseShift.AdvancedParameters.Contains("Starter") Then
                                            bolBaseShiftwithExpected = True
                                        End If
                                    End If
                                    If (Not isHolidays And Not bolRemoveholidays) Or (baseShiftId <> -1 And Not bolBaseShiftwithExpected) Then
                                        ' Eliminar los datos de horarios por horas y starter
                                        oRow("ExpectedWorkingHours") = DBNull.Value
                                        oRow("LayersDefinition") = DBNull.Value
                                    End If

                                    oRow("Status") = 0
                                    oRow("TaskStatus") = 0
                                    oRow("GUID") = ""

                                    If bolIsNew OrElse roTimeStamps.CheckIfScheduleHasChanged(oRow) Then
                                        oRow("Timestamp") = Now
                                    End If

                                    If tb.Rows.Count = 0 Then
                                        tb.Rows.Add(oRow)
                                    End If

                                    ad.Update(tb)

                                End If

                                bolRet = (oState.Result = EmployeeResultEnum.NoError)

                                If bolRet And bAudit Then
                                    'Auditamos
                                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                    oState.AddAuditParameter(tbParameters, "{IDShift}", intIDShift1, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{ShiftName}", Shift.roShift.GetName(intIDShift1, oShiftState), "", 1)
                                    oState.AddAuditParameter(tbParameters, "{EmployeeID}", IDEmployee, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", roBusinessSupport.GetEmployeeName(IDEmployee, Nothing), "", 1)
                                    oState.AddAuditParameter(tbParameters, "{ShiftDate}", xShiftDate, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{IDAssignment}", intIDAssignment, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{AssignmentName}", Assignment.roAssignment.GetName(intIDAssignment, Nothing), "", 1)
                                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tSchedulerPlanification, roBusinessSupport.GetEmployeeName(IDEmployee, Nothing), tbParameters, -1)
                                End If

                                If bolNotify And bolAssign Then
                                    ' Notificamos el cambio
                                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                                End If

                                If bolAssign Then

                                    ' Marcamos para recálculo las dotaciones planificadas para el empleado y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                    Dim oSchedulerState As New Scheduler.roSchedulerState
                                    roBusinessState.CopyTo(oState, oSchedulerState)
                                    bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, IDEmployee, , xShiftDate, bolNotify)
                                    roBusinessState.CopyTo(oSchedulerState, oState)

                                    If bolRet Then
                                        ' Marcamos para recálculo las reales planificadas para el empleado y fecha, y no notificamos al servidor.
                                        bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, IDEmployee, , xShiftDate, bolNotify)
                                        roBusinessState.CopyTo(oSchedulerState, oState)
                                    End If

                                End If
                            Catch ex As DbException
                                oState.UpdateStateInfo(ex, "roEmployees::AssignShift")
                            Catch ex As Exception
                                oState.UpdateStateInfo(ex, "roEmployees::AssignShift")
                            End Try
                        Else
                            oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                        End If
                    Else
                        oState.Result = EmployeeResultEnum.EmployeeNoActiveContract
                    End If
                Else
                    oState.Result = EmployeeResultEnum.AccessDenied
                End If
            End If

            Return bolRet

        End Function

        Public Shared Function AssignShiftEx(ByVal IDEmployee As Integer, ByVal xShiftDate As Date,
                            ByVal intIDShift1 As Integer, ByVal intIDShift2 As Integer, ByVal intIDShift3 As Integer, ByVal intIDShift4 As Integer,
                            ByVal _StartShift1 As DateTime, ByVal _StartShift2 As DateTime, ByVal _StartShift3 As DateTime, ByVal _StartShift4 As DateTime,
                            ByVal intIDAssignment As Integer,
                            ByRef _LockedDayAction As LockedDayAction, ByRef _CoverageDayAction As LockedDayAction, ByRef oState As Employee.roEmployeeState, ByVal bolHRSchedulingInstalled As Boolean, ByVal xFirstDate As Date, ByVal bolExistBusinessGroup As Boolean,
                            Optional ByVal bolCheckPermission As Boolean = True, Optional ByVal bolNotify As Boolean = True,
                            Optional ByVal bAudit As Boolean = False,
                            Optional ByVal _ShiftPermissionAction As ShiftPermissionAction = ShiftPermissionAction.ContinueAll,
                            Optional ByVal intIDShiftBase As Integer = -2, Optional ByVal intIDAssignmentBase As Integer = -2,
                            Optional ByVal xStartShiftBase As DateTime = Nothing, Optional ByVal isHolidaysBase As Boolean = False,
                            Optional ByVal xStartFlexible1 As DateTime = Nothing, Optional ByVal xEndFlexible1 As DateTime = Nothing, Optional ByVal cColorFlexible1 As Integer = 0, Optional ByVal sFlexibleShiftName1 As String = "",
                            Optional ByVal xStartFlexibleBase As DateTime = Nothing, Optional ByVal xEndFlexibleBase As DateTime = Nothing, Optional ByVal cColorFlexibleBase As Integer = 0, Optional ByVal sFlexibleShiftNameBase As String = "") As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False

            Dim oShiftState As New Shift.roShiftState()
            oShiftState.IDPassport = oState.IDPassport

            Dim bContinuar As Boolean = True

            If bolExistBusinessGroup Then

                If Not Shift.roShift.ShiftIsAllowed(oShiftState, intIDShift1, intIDShift2, intIDShift3, intIDShift4) Then

                    If _ShiftPermissionAction = ShiftPermissionAction.None Or
               _ShiftPermissionAction = ShiftPermissionAction.StopAll Then

                        oState.Result = EmployeeResultEnum.ShiftWithoutPermission

                    ElseIf _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Or
                   _ShiftPermissionAction = ShiftPermissionAction.ContinueAll Then

                        oState.Result = EmployeeResultEnum.NoError

                    End If

                    bContinuar = False

                End If
            End If

            If bContinuar Then
                Dim bolHasPermission As Boolean = True

                If bolCheckPermission Then
                    ' Verificamos los permisos del pasaporte actual sobre la planificación
                    bolHasPermission = WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, IDEmployee, xShiftDate, )

                End If
                If bolHasPermission Then

                    'Solamente asigna el horario si este tiene contrato
                    If roBusinessSupport.EmployeeWithContract(IDEmployee, oState, xShiftDate) Then
                        Dim xFreezingDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, False, oState)
                        'If xFirstDate > xFreezingDateEmployee Then xFreezingDateEmployee = xFirstDate

                        If Not (xShiftDate <= CDate(xFreezingDateEmployee)) Then

                            Try

                                Dim bolAssign As Boolean = True

                                Dim strSQL As String
                                strSQL = "@SELECT# * FROM DailySchedule WHERE"
                                strSQL &= " DailySchedule.IDEmployee = " & IDEmployee & " AND"
                                strSQL &= " DailySchedule.Date = " & Any2Time(xShiftDate).SQLSmallDateTime

                                Dim tb As New DataTable
                                Dim cmd As DbCommand = CreateCommand(strSQL)
                                Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)

                                ad.Fill(tb)

                                Dim bolModifiedShift As Boolean = False

                                Dim oRow As DataRow

                                If tb.Rows.Count = 0 Then
                                    oRow = tb.NewRow
                                    bolIsNew = True
                                    oRow("IDEmployee") = IDEmployee
                                    oRow("Date") = xShiftDate
                                    oRow("LockedDay") = False
                                Else
                                    oRow = tb.Rows(0)
                                    If Any2Boolean(oRow("LockedDay")) Then
                                        ' El día a asignar está bloqueado.
                                        Select Case _LockedDayAction
                                            Case LockedDayAction.None
                                                ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                oState.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                                bolAssign = False
                                            Case LockedDayAction.ReplaceFirst
                                                ' Se tiene que asignar igualmente
                                                bolAssign = True
                                                _LockedDayAction = LockedDayAction.None
                                            Case LockedDayAction.ReplaceAll
                                                ' Se tiene que asignar igualmente
                                                bolAssign = True
                                            Case LockedDayAction.NoReplaceFirst
                                                ' No se tiene que asignar
                                                bolAssign = False
                                                _LockedDayAction = LockedDayAction.None
                                            Case LockedDayAction.NoReplaceAll
                                                ' No se tiene que asignar
                                                bolAssign = False
                                        End Select
                                    End If

                                    ' Verificamos que no este asignado ya algun presupuesto
                                    ' en ese caso no se debe asignar nada ese dia
                                    If bolAssign Then
                                        If Any2Long(oRow("IDDailyBudgetPosition")) > 0 Then
                                            ' No se tiene que asignar
                                            bolAssign = False
                                        End If
                                    End If

                                End If

                                ' Verificamos que los horarios a asignar sean correctos (no haya duplicados)
                                If bolAssign Then

                                    Dim _IDShift1 As Integer = 0
                                    If intIDShift1 > 0 Then
                                        _IDShift1 = intIDShift1
                                    ElseIf intIDShift1 = 0 Then
                                        _IDShift1 = Any2Integer(oRow("IDShift1"))
                                        If Not IsDBNull(oRow("StartShift1")) Then
                                            _StartShift1 = oRow("StartShift1")
                                        Else
                                            _StartShift1 = Nothing
                                        End If
                                    End If

                                    Dim _IDShift2 As Integer = 0
                                    If intIDShift2 > 0 Then
                                        _IDShift2 = intIDShift2
                                    ElseIf intIDShift2 = 0 Then
                                        _IDShift2 = Any2Integer(oRow("IDShift2"))
                                        If Not IsDBNull(oRow("StartShift2")) Then
                                            _StartShift2 = oRow("StartShift2")
                                        Else
                                            _StartShift2 = Nothing
                                        End If
                                    End If
                                    Dim _IDShift3 As Integer = 0
                                    If intIDShift3 > 0 Then
                                        _IDShift3 = intIDShift3
                                    ElseIf intIDShift3 = 0 Then
                                        _IDShift3 = Any2Integer(oRow("IDShift3"))
                                        If Not IsDBNull(oRow("StartShift3")) Then
                                            _StartShift3 = oRow("StartShift3")
                                        Else
                                            _StartShift3 = Nothing
                                        End If
                                    End If
                                    Dim _IDShift4 As Integer = 0
                                    If intIDShift4 > 0 Then
                                        _IDShift4 = intIDShift4
                                    ElseIf intIDShift4 = 0 Then
                                        _IDShift4 = Any2Integer(oRow("IDShift4"))
                                        If Not IsDBNull(oRow("StartShift4")) Then
                                            _StartShift4 = oRow("StartShift4")
                                        Else
                                            _StartShift4 = Nothing
                                        End If
                                    End If

                                    Dim bCompact As Boolean = False
                                    ' Miramos que los horarios planificados no estén ya planificados para ese empleado y día, pero en otra posición ... (alternativos)
                                    If _IDShift1 > 0 Then 'And ((_IDShift1 = _IDShift2 And _StartShift1 = _StartShift2) Or (_IDShift1 = _IDShift3 And _StartShift1 = _StartShift3) Or (_IDShift1 = _IDShift4 And _StartShift1 = _StartShift4)) Then
                                        If _IDShift1 = _IDShift2 AndAlso _StartShift1 = _StartShift2 Then
                                            ' Eliminio el horario 2
                                            intIDShift2 = -1
                                            _IDShift2 = 0
                                            _StartShift2 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift1 = _IDShift3 AndAlso _StartShift1 = _StartShift3 Then
                                            ' Eliminio el horario 3
                                            intIDShift3 = -1
                                            _IDShift3 = 0
                                            _StartShift3 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift1 = _IDShift4 AndAlso _StartShift1 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                        'bolAssign = False
                                    End If
                                    If bolAssign And _IDShift2 > 0 Then 'And ((_IDShift2 = _IDShift3 And _StartShift2 = _StartShift3) Or (_IDShift2 = _IDShift4 And _StartShift2 = _StartShift4)) Then
                                        If _IDShift2 = _IDShift3 And _StartShift2 = _StartShift3 Then
                                            ' Eliminio el horario 3
                                            intIDShift3 = -1
                                            _IDShift3 = 0
                                            _StartShift3 = Nothing
                                            bCompact = True
                                        End If
                                        If _IDShift2 = _IDShift4 And _StartShift2 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                        'bolAssign = False
                                    End If
                                    If bolAssign And _IDShift3 > 0 Then 'And ((_IDShift3 = _IDShift2 And _StartShift3 = _StartShift2) Or (_IDShift3 = _IDShift4 And _StartShift3 = _StartShift4)) Then
                                        If _IDShift3 = _IDShift4 And _StartShift3 = _StartShift4 Then
                                            ' Eliminio el horario 4
                                            intIDShift4 = -1
                                            _IDShift4 = 0
                                            _StartShift4 = Nothing
                                            bCompact = True
                                        End If
                                        'bolAssign = False
                                    End If
                                    'If bolAssign And _IDShift4 > 0 And ((_IDShift4 = _IDShift2 And _StartShift4 = _StartShift2) Or (_IDShift4 = _IDShift3 And _StartShift4 = _StartShift3)) Then
                                    'bolAssign = False
                                    'End If

                                    'TODO: Compactamos si es necesario (se borró algún alternativo

                                    If Not bolAssign Then oState.Result = EmployeeResultEnum.ShiftAlreadyAssigned

                                End If

                                Dim baseShiftId As Integer = -1
                                Dim baseStartDate As Date = Nothing
                                Dim dBaseStartFlexibleDate As Date = xStartFlexibleBase
                                Dim dBaseEndFlexibleDate As Date = xEndFlexibleBase
                                Dim iBaseFlexibleColor As Integer = cColorFlexibleBase
                                Dim sBaseFlexibleName As String = sFlexibleShiftNameBase
                                Dim dStartFlexibleDate As Date = xStartFlexible1
                                Dim dEndFlexibleDate As Date = xEndFlexible1
                                Dim iFlexibleColor As Integer = cColorFlexible1
                                Dim sFlexibleName As String = sFlexibleShiftName1

                                Dim isHolidays As Boolean = False
                                Dim baseAssignmentId As Integer = -1

                                'Si me pasan parametros para el turno base, es que vamos a copiarlo
                                If (intIDAssignmentBase = -2 AndAlso intIDShiftBase = -2) Then
                                    'No se permite asignar un horario de vacaciones a un dia sin planificar
                                    If bolAssign And intIDShift1 > 0 And Shift.roShift.IsHolidays(intIDShift1) And Any2Integer(oRow("IDShift1")) = 0 Then
                                        bolAssign = False

                                        If Not bolAssign Then oState.Result = EmployeeResultEnum.NoBaseShiftAssigned
                                    End If

                                    'Miramos si el horario que tiene asignado actualmente ya es de vacaciones
                                    Dim actuallyHolidays As Boolean = roTypes.Any2Boolean(oRow("IsHolidays"))
                                    Dim idHolidayShift As Integer = 0
                                    If actuallyHolidays Then
                                        idHolidayShift = Any2Integer(oRow("IDShift1"))
                                    End If

                                    'Miramos si el horario que nos quieren asignar es de vacaciones
                                    Dim intHolidayShift As Integer = 0
                                    If (bolAssign AndAlso Shift.roShift.IsHolidays(intIDShift1)) Then

                                        Dim vacShift As Shift.roShift = New Shift.roShift(intIDShift1, oShiftState, False)
                                        If vacShift.AreWorkingDays Then
                                            Dim baseShift As Shift.roShift = Nothing

                                            If actuallyHolidays Then
                                                baseShift = New Shift.roShift(Any2Integer(oRow("IDShiftBase")), oShiftState, False)
                                            Else
                                                baseShift = New Shift.roShift(Any2Integer(oRow("IDShift1")), oShiftState, False)
                                            End If

                                            'Si el horario base no tiene horas y el horario de vacaciones es solo para laborables no dejamos asignar
                                            If baseShift.ExpectedWorkingHours = 0 Then
                                                bolAssign = False
                                                oState.Result = EmployeeResultEnum.NoWorkingDay
                                            End If

                                        End If

                                        intHolidayShift = intIDShift1
                                        intIDShift1 = 0
                                    End If

                                    If bolAssign Then
                                        If intHolidayShift > 0 Then 'Si nos quieren asignar un horario de vacaciones
                                            If actuallyHolidays Then 'Si ya estamos en vacaciones guardamos el shift 1 y mantenemos el base ya existente
                                                intIDShift1 = intHolidayShift
                                                baseShiftId = Any2Integer(oRow("IDShiftBase"))

                                                If Not IsDBNull(oRow("StartShiftBase")) Then
                                                    baseStartDate = oRow("StartShiftBase")
                                                Else
                                                    baseStartDate = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                    dBaseStartFlexibleDate = oRow("StartFlexibleBase")
                                                Else
                                                    dBaseStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                    dBaseEndFlexibleDate = oRow("EndFlexibleBase")
                                                Else
                                                    dBaseEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                    sBaseFlexibleName = oRow("ShiftNameBase")
                                                Else
                                                    sBaseFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                    iBaseFlexibleColor = oRow("ShiftColorBase")
                                                Else
                                                    iBaseFlexibleColor = Nothing
                                                End If

                                                baseAssignmentId = Any2Integer(oRow("IDAssignmentBase"))

                                                If idHolidayShift <> intIDShift1 Then
                                                    oRow("TimestampHolidays") = Date.Now
                                                End If
                                            Else 'Si no estoy en vacaciones pasamos los datos actuales al base y guardamos el shift1
                                                baseShiftId = Any2Integer(oRow("IDShift1"))

                                                If Not IsDBNull(oRow("StartShift1")) Then
                                                    baseStartDate = oRow("StartShift1")
                                                Else
                                                    baseStartDate = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexible1")) Then
                                                    dBaseStartFlexibleDate = oRow("StartFlexible1")
                                                Else
                                                    dBaseStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexible1")) Then
                                                    dBaseEndFlexibleDate = oRow("EndFlexible1")
                                                Else
                                                    dBaseEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftName1")) Then
                                                    sBaseFlexibleName = oRow("ShiftName1")
                                                Else
                                                    sBaseFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColor1")) Then
                                                    iBaseFlexibleColor = oRow("ShiftColor1")
                                                Else
                                                    iBaseFlexibleColor = Nothing
                                                End If

                                                baseAssignmentId = Any2Integer(oRow("IDAssignment"))
                                                intIDShift1 = intHolidayShift

                                                oRow("TimestampHolidays") = Date.Now
                                            End If
                                            isHolidays = True
                                        Else 'Si asignamos un horario que no es de vacaciones

                                            Dim Aux_intIDShift1 As Integer = intIDShift1
                                            Dim Aux__StartShift1 As Date = _StartShift1
                                            Dim Aux_StartFlexible1 As Date = dStartFlexibleDate
                                            Dim Aux_EndFlexible1 As Date = dEndFlexibleDate
                                            Dim Aux_ColorFlexible1 As Integer = iFlexibleColor
                                            Dim Aux_FlexibleShiftName1 As String = sFlexibleName

                                            If intIDShift1 <= 0 Then 'Si pasamos un idshift1 como 0, significa que vamos a quitar el horario de vacaciones. Pasamos la info del base al shift1
                                                intIDShift1 = Any2Integer(oRow("IDShiftBase"))
                                                If Not IsDBNull(oRow("StartShiftBase")) Then
                                                    _StartShift1 = oRow("StartShiftBase")
                                                Else
                                                    _StartShift1 = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexibleBase")) Then
                                                    dStartFlexibleDate = oRow("StartFlexibleBase")
                                                Else
                                                    dStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexibleBase")) Then
                                                    dEndFlexibleDate = oRow("EndFlexibleBase")
                                                Else
                                                    dEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftNameBase")) Then
                                                    sFlexibleName = oRow("ShiftNameBase")
                                                Else
                                                    sFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColorBase")) Then
                                                    iFlexibleColor = oRow("ShiftColorBase")
                                                Else
                                                    iFlexibleColor = Nothing
                                                End If

                                                intIDAssignment = Any2Integer(oRow("IDAssignmentBase"))

                                                'Registramos que se ha eliminado un dia que estaba con vacaciones
                                                Dim oProgrammedManager As New roProgrammedHolidayManager
                                                Dim dNow As DateTime = Date.Now
                                                oProgrammedManager.RegisterDeleteProgrammedHoliday(IDEmployee, , xShiftDate, dNow)
                                                oRow("TimestampHolidays") = dNow
                                            End If
                                            ' ***********************
                                            ' Si asignamos un horario normal a un dia que tiene vacaciones ya asignadas
                                            ' debemos reemplazar el horario base por el que nos pasan
                                            If Aux_intIDShift1 > 0 And actuallyHolidays Then
                                                isHolidays = True
                                                baseShiftId = Aux_intIDShift1
                                                baseStartDate = Aux__StartShift1
                                                dBaseStartFlexibleDate = Aux_StartFlexible1
                                                dBaseEndFlexibleDate = Aux_EndFlexible1
                                                iBaseFlexibleColor = Aux_ColorFlexible1
                                                sBaseFlexibleName = Aux_FlexibleShiftName1

                                                intIDShift1 = Any2Integer(oRow("IDShift1"))
                                                If Not IsDBNull(oRow("StartShift1")) Then
                                                    _StartShift1 = oRow("StartShift1")
                                                Else
                                                    _StartShift1 = Nothing
                                                End If

                                                If Not IsDBNull(oRow("StartFlexible1")) Then
                                                    dStartFlexibleDate = oRow("StartFlexible1")
                                                Else
                                                    dStartFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("EndFlexible1")) Then
                                                    dEndFlexibleDate = oRow("EndFlexible1")
                                                Else
                                                    dEndFlexibleDate = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftName1")) Then
                                                    sFlexibleName = oRow("ShiftName1")
                                                Else
                                                    sFlexibleName = Nothing
                                                End If
                                                If Not IsDBNull(oRow("ShiftColor1")) Then
                                                    iFlexibleColor = oRow("ShiftColor1")
                                                Else
                                                    iFlexibleColor = Nothing
                                                End If
                                            Else
                                                'Al asignar un horario borramos la información que contiene el base
                                                baseShiftId = -1
                                                baseStartDate = Nothing
                                                baseAssignmentId = -1

                                                dBaseStartFlexibleDate = Nothing
                                                dBaseEndFlexibleDate = Nothing
                                                iBaseFlexibleColor = 0
                                                sBaseFlexibleName = Nothing

                                                isHolidays = False

                                            End If
                                        End If
                                    End If
                                Else
                                    baseShiftId = intIDShiftBase
                                    baseStartDate = xStartShiftBase

                                    dBaseStartFlexibleDate = xStartFlexibleBase
                                    dBaseEndFlexibleDate = xEndFlexibleBase
                                    iBaseFlexibleColor = cColorFlexibleBase
                                    sBaseFlexibleName = sFlexibleShiftNameBase

                                    isHolidays = isHolidaysBase
                                    baseAssignmentId = intIDAssignmentBase
                                End If

                                Dim bolAssignmentChanged As Boolean = False

                                ' Verificamos que el puesto sea compatible con el empleado y el horario
                                If Not isHolidays Then 'Si no estamos en vacaciones comprobamos la idoniedad del puesto si procede
                                    ' Verificamos que el puesto sea compatible con el empleado y el horario
                                    If bolAssign Then

                                        Dim _IDAssignment As Integer = 0
                                        If intIDAssignment > 0 Then
                                            _IDAssignment = intIDAssignment
                                        ElseIf intIDAssignment = 0 Then
                                            _IDAssignment = Any2Integer(oRow("IDAssignment"))
                                        End If

                                        Dim _IDShift1 As Integer = 0
                                        If intIDShift1 > 0 Then
                                            _IDShift1 = intIDShift1
                                        ElseIf intIDShift1 = 0 Then
                                            _IDShift1 = Any2Integer(oRow("IDShift1"))
                                        End If

                                        If _IDShift1 > 0 And _IDAssignment > 0 Then

                                            ' Si el empleado no tiene este puesto asignado en su definición, se borra el puesto de la DailySchedule
                                            If Not roBusinessSupport.ExistEmployeeAssignment(IDEmployee, _IDAssignment, oState) Then
                                                intIDAssignment = -1
                                            End If

                                            ' Si el horario principal no tiene este puesto asignado, se borra el puesto
                                            roBusinessState.CopyTo(oState, oShiftState)
                                            If Not Shift.roShiftAssignment.ExistShiftAssignment(_IDShift1, _IDAssignment, oShiftState) Then
                                                intIDAssignment = -1
                                            End If
                                            roBusinessState.CopyTo(oShiftState, oState)
                                        Else
                                            intIDAssignment = -1
                                        End If

                                    End If

                                    If bolAssign Then

                                        ' Si se está modificando el puesto, verificamos que no haya cobertura
                                        If intIDAssignment > 0 Then
                                            bolAssignmentChanged = (Any2Integer(oRow("IDAssignment")) <> intIDAssignment)
                                        ElseIf intIDAssignment = -1 Then
                                            bolAssignmentChanged = (Not IsDBNull(oRow("IDAssignment")))
                                        End If

                                        If bolAssignmentChanged AndAlso (Any2Boolean(oRow("IsCovered")) OrElse Any2Integer(oRow("IDEmployeeCovered")) > 0) Then

                                            Select Case _CoverageDayAction
                                                Case LockedDayAction.None
                                                    ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                    oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay
                                                    bolAssign = False
                                                Case LockedDayAction.ReplaceFirst
                                                    ' Se tiene que asignar igualmente
                                                    bolAssign = True
                                                    _CoverageDayAction = LockedDayAction.None
                                                Case LockedDayAction.ReplaceAll
                                                    ' Se tiene que asignar igualmente
                                                    bolAssign = True
                                                Case LockedDayAction.NoReplaceFirst
                                                    ' No se tiene que asignar
                                                    bolAssign = False
                                                    _CoverageDayAction = LockedDayAction.None
                                                Case LockedDayAction.NoReplaceAll
                                                    ' No se tiene que asignar
                                                    bolAssign = False
                                            End Select

                                        End If

                                    End If
                                Else
                                    'Si estoy de vacaciones pongo el idassignment a 0, anteriormente ya lo he guardado en el base si procede
                                    intIDAssignment = -1
                                End If

                                If bolAssign Then
                                    If intIDShift1 > 0 Then
                                        Dim intOldShift As Integer = roTypes.Any2Integer(oRow("IDShift1"))
                                        If IsDBNull(oRow("IDShift1")) OrElse roTypes.Any2Double(oRow("IDShift1")) = 0 Then
                                            'Xaavi: Pueden cambiar el horario flexible de Starter sin haber cambiado el id del horario. Pero en Starter no hay notificaciones ...
                                            bolModifiedShift = False
                                        Else
                                            ' Si previamente tiene asignado un horario,
                                            ' en caso necesario hay que revisar el nuevo horario asignado
                                            ' para la notificacion de horario asignado
                                            bolModifiedShift = True
                                            If intOldShift <> intIDShift1 Then
                                                oRow("IDPreviousShift") = intOldShift
                                            End If
                                        End If

                                        oRow("IDShift1") = intIDShift1
                                        If _StartShift1 <> Nothing Then
                                            oRow("StartShift1") = _StartShift1
                                        Else
                                            oRow("StartShift1") = DBNull.Value
                                        End If

                                        If dStartFlexibleDate <> Nothing Then
                                            oRow("StartFlexible1") = dStartFlexibleDate
                                        Else
                                            oRow("StartFlexible1") = DBNull.Value
                                        End If

                                        If dEndFlexibleDate <> Nothing Then
                                            oRow("EndFlexible1") = dEndFlexibleDate
                                        Else
                                            oRow("EndFlexible1") = DBNull.Value
                                        End If

                                        If iFlexibleColor <> Nothing AndAlso iFlexibleColor > 0 Then
                                            oRow("ShiftColor1") = iFlexibleColor
                                        Else
                                            oRow("ShiftColor1") = DBNull.Value
                                        End If

                                        If sFlexibleName <> Nothing AndAlso sFlexibleName <> String.Empty Then
                                            oRow("ShiftName1") = sFlexibleName
                                        Else
                                            oRow("ShiftName1") = DBNull.Value
                                        End If

                                        ' Verificamos si hay que revisar la notificacion de Asignacion de horario
                                        Dim oNotificationState As New Notifications.roNotificationState(-1)
                                        Dim oAssignedShiftNotifications As Generic.List(Of Notifications.roNotification) = Notifications.roNotification.GetNotifications("IDType = 51 And Activated=1", oNotificationState,, True)

                                        If oAssignedShiftNotifications IsNot Nothing AndAlso oAssignedShiftNotifications.Count > 0 AndAlso bolModifiedShift AndAlso xShiftDate > DateTime.Now.Date Then
                                            Notifications.roNotification.ExecuteAssignedShiftNotification(oNotificationState, oAssignedShiftNotifications, IDEmployee, xShiftDate, intIDShift1, intOldShift, False)
                                        End If

                                    ElseIf intIDShift1 = -1 Then
                                        oRow("IDShift1") = DBNull.Value
                                        oRow("StartShift1") = DBNull.Value

                                        oRow("StartFlexible1") = DBNull.Value
                                        oRow("EndFlexible1") = DBNull.Value
                                        oRow("ShiftColor1") = DBNull.Value
                                        oRow("Shiftname1") = DBNull.Value
                                    End If
                                    If intIDShift2 > 0 Then
                                        oRow("IDShift2") = intIDShift2
                                        If _StartShift2 <> Nothing Then
                                            oRow("StartShift2") = _StartShift2
                                        Else
                                            oRow("StartShift2") = DBNull.Value
                                        End If
                                    ElseIf intIDShift2 = -1 Then
                                        oRow("IDShift2") = DBNull.Value
                                        oRow("StartShift2") = DBNull.Value
                                    End If
                                    If intIDShift3 > 0 Then
                                        oRow("IDShift3") = intIDShift3
                                        If _StartShift3 <> Nothing Then
                                            oRow("StartShift3") = _StartShift3
                                        Else
                                            oRow("StartShift3") = DBNull.Value
                                        End If
                                    ElseIf intIDShift3 = -1 Then
                                        oRow("IDShift3") = DBNull.Value
                                        oRow("StartShift3") = DBNull.Value
                                    End If
                                    If intIDShift4 > 0 Then
                                        oRow("IDShift4") = intIDShift4
                                        If _StartShift4 <> Nothing Then
                                            oRow("StartShift4") = _StartShift4
                                        Else
                                            oRow("StartShift4") = DBNull.Value
                                        End If
                                    ElseIf intIDShift4 = -1 Then
                                        oRow("IDShift4") = DBNull.Value
                                        oRow("StartShift4") = DBNull.Value
                                    End If

                                    If intIDAssignment > 0 Then
                                        oRow("IDAssignment") = intIDAssignment
                                    ElseIf intIDAssignment = -1 Then
                                        oRow("IDAssignment") = DBNull.Value
                                    End If

                                    If baseShiftId > 0 Then
                                        oRow("IDShiftBase") = baseShiftId
                                        If baseStartDate <> Nothing Then
                                            oRow("StartShiftBase") = baseStartDate
                                        Else
                                            oRow("StartShiftBase") = DBNull.Value
                                        End If

                                        If dBaseStartFlexibleDate <> Nothing Then
                                            oRow("StartFlexibleBase") = dBaseStartFlexibleDate
                                        Else
                                            oRow("StartFlexibleBase") = DBNull.Value
                                        End If
                                        If dBaseEndFlexibleDate <> Nothing Then
                                            oRow("EndFlexibleBase") = dBaseEndFlexibleDate
                                        Else
                                            oRow("EndFlexibleBase") = DBNull.Value
                                        End If
                                        If iBaseFlexibleColor <> Nothing AndAlso iBaseFlexibleColor > 0 Then
                                            oRow("ShiftColorBase") = iBaseFlexibleColor
                                        Else
                                            oRow("ShiftColorBase") = DBNull.Value
                                        End If
                                        If sBaseFlexibleName <> Nothing AndAlso sBaseFlexibleName <> String.Empty Then
                                            oRow("ShiftNameBase") = sBaseFlexibleName
                                        Else
                                            oRow("ShiftNameBase") = DBNull.Value
                                        End If

                                    ElseIf baseShiftId = -1 Then
                                        oRow("IDShiftBase") = DBNull.Value
                                        oRow("StartShiftBase") = DBNull.Value

                                        oRow("StartFlexibleBase") = DBNull.Value
                                        oRow("EndFlexibleBase") = DBNull.Value
                                        oRow("ShiftColorBase") = DBNull.Value
                                        oRow("ShiftNameBase") = DBNull.Value
                                    End If

                                    If baseAssignmentId > 0 Then
                                        oRow("IDAssignmentBase") = baseAssignmentId
                                    ElseIf baseAssignmentId = -1 Then
                                        oRow("IDAssignmentBase") = DBNull.Value
                                    End If

                                    If isHolidays Then
                                        oRow("IsHolidays") = isHolidays
                                    Else
                                        oRow("IsHolidays") = DBNull.Value
                                    End If

                                    oRow("IDDailyBudgetPosition") = DBNull.Value

                                    'Reseteamos los valores de LayersDefinition y ExpectedWorkingHours para que lo coja por defecto del nuevo horario asignado
                                    oRow("LayersDefinition") = DBNull.Value
                                    oRow("ExpectedWorkingHours") = DBNull.Value

                                    If bolAssignmentChanged Then
                                        ' Si se está modificando el puesto, borramos la posible cobertura
                                        If Any2Boolean(oRow("IsCovered")) Then

                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                             "SET IDAssignment = OldIDAssignment, " &
                                                 "OldIDAssignment = NULL, " &
                                                 "IDEmployeeCovered = NULL, " &
                                                 "Status = 0, [GUID] = '' " &
                                             "WHERE IDEmployeeCovered = " & IDEmployee.ToString & " AND " &
                                                   "Date = " & Any2Time(xShiftDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)

                                        ElseIf Any2Integer(oRow("IDEmployeeCovered")) > 0 Then

                                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                             "SET IsCovered = 0, " &
                                                 "Status = 0, [GUID] = '' " &
                                             "WHERE IDEmployee = " & Any2Integer(oRow("IDEmployeeCovered")) & " AND " &
                                                   "Date = " & Any2Time(xShiftDate).SQLSmallDateTime
                                            ExecuteSql(strSQL)

                                        End If
                                        oRow("IsCovered") = DBNull.Value
                                        oRow("OldIDAssignment") = DBNull.Value
                                        oRow("IDEmployeeCovered") = DBNull.Value

                                    End If

                                    oRow("Status") = 0
                                    oRow("TaskStatus") = 0
                                    oRow("GUID") = ""

                                    If bolIsNew OrElse roTimeStamps.CheckIfScheduleHasChanged(oRow) Then
                                        oRow("Timestamp") = Now
                                    End If

                                    If tb.Rows.Count = 0 Then
                                        tb.Rows.Add(oRow)
                                    End If

                                    ad.Update(tb)
                                End If

                                bolRet = (oState.Result = EmployeeResultEnum.NoError)

                                If bolRet And bAudit Then
                                    'Auditamos
                                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                    oState.AddAuditParameter(tbParameters, "{IDShift}", intIDShift1, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{ShiftName}", Shift.roShift.GetName(intIDShift1, oShiftState), "", 1)
                                    oState.AddAuditParameter(tbParameters, "{EmployeeID}", IDEmployee, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", roBusinessSupport.GetEmployeeName(IDEmployee, Nothing), "", 1)
                                    oState.AddAuditParameter(tbParameters, "{ShiftDate}", xShiftDate, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{IDAssignment}", intIDAssignment, "", 1)
                                    oState.AddAuditParameter(tbParameters, "{AssignmentName}", Assignment.roAssignment.GetName(intIDAssignment, Nothing), "", 1)
                                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tSchedulerPlanification, roBusinessSupport.GetEmployeeName(IDEmployee, Nothing), tbParameters, -1)
                                End If

                                If bolAssign And bolHRSchedulingInstalled Then

                                    ' Marcamos para recálculo las dotaciones planificadas para el empleado y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                    Dim oSchedulerState As New Scheduler.roSchedulerState
                                    roBusinessState.CopyTo(oState, oSchedulerState)
                                    bolRet = Scheduler.roDailyCoverage.RecalculateEx(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, bolHRSchedulingInstalled, IDEmployee, , xShiftDate, False)
                                    roBusinessState.CopyTo(oSchedulerState, oState)

                                    If bolRet Then
                                        ' Marcamos para recálculo las reales planificadas para el empleado y fecha, y no notificamos al servidor.
                                        bolRet = Scheduler.roDailyCoverage.RecalculateEx(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, bolHRSchedulingInstalled, IDEmployee, , xShiftDate, False)
                                        roBusinessState.CopyTo(oSchedulerState, oState)
                                    End If

                                End If
                            Catch ex As Exception
                                oState.UpdateStateInfo(ex, "roEmployees::AssignShiftEx")
                            End Try
                        Else
                            oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                        End If
                    Else
                        oState.Result = EmployeeResultEnum.EmployeeNoActiveContract
                    End If
                Else
                    oState.Result = EmployeeResultEnum.AccessDenied
                End If
            End If

            Return bolRet

        End Function

        Public Shared Function AssignAlterShift(ByVal intIDEmployee As Integer, ByVal xShiftDate As DateTime, ByVal intIDAlterShift As Integer, ByVal xAlterStartShift As DateTime, ByRef _LockedDayAction As LockedDayAction, ByRef oState As Employee.roEmployeeState, Optional ByVal bolCheckPermission As Boolean = True) As Boolean
            '
            ' Asigna el horario como el primer alternativo no informado del empleado y fecha.
            ' Si no hay horario principal informado o todos los alternativos ya están informados devuelve 'false'
            '
            Dim bolRet As Boolean = False

            Dim bolHasPermission As Boolean = True
            If bolCheckPermission Then
                ' Verificamos los permisos del pasaporte actual sobre la planificación
                bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intIDEmployee)
            End If

            If bolHasPermission Then

                'Solamente asigna el horario si este tiene contrato
                If roBusinessSupport.EmployeeWithContract(intIDEmployee, oState, xShiftDate) Then

                    If Not roBusinessSupport.InPeriodOfFreezing(xShiftDate, intIDEmployee, oState) Then

                        Try

                            Dim strSQL As String
                            strSQL = "@SELECT# * FROM DailySchedule " &
                                     "WHERE DailySchedule.IDEmployee = " & intIDEmployee & " AND " &
                                           "DailySchedule.Date = " & Any2Time(xShiftDate.Date).SQLSmallDateTime

                            Dim tb As New DataTable
                            Dim cmd As DbCommand = CreateCommand(strSQL)
                            Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)
                            ad.Fill(tb)

                            If tb.Rows.Count = 1 Then

                                Dim oRow As DataRow = tb.Rows(0)

                                Dim bolAssign As Boolean = True

                                If Any2Boolean(oRow("LockedDay")) Then
                                    ' El día a asignar está bloqueado
                                    Select Case _LockedDayAction
                                        Case LockedDayAction.None
                                            ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                            oState.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                            bolAssign = False
                                        Case LockedDayAction.ReplaceFirst
                                            ' Se tiene que asignar igualmente
                                            bolAssign = True
                                            _LockedDayAction = LockedDayAction.None
                                        Case LockedDayAction.ReplaceAll
                                            ' Se tiene que asignar igualmente
                                            bolAssign = True
                                        Case LockedDayAction.NoReplaceFirst
                                            ' No se tiene que asignar
                                            bolAssign = False
                                            _LockedDayAction = LockedDayAction.None
                                        Case LockedDayAction.NoReplaceAll
                                            ' No se tiene que asignar
                                            bolAssign = False
                                    End Select
                                End If

                                If bolAssign Then

                                    If Not IsDBNull(oRow("IDShift1")) AndAlso oRow("IDShift1") > 0 Then

                                        Dim intIDShift1 As Integer = oRow("IDShift1")
                                        Dim intIDShift2 As Integer = 0
                                        Dim intIDShift3 As Integer = 0
                                        Dim intIDShift4 As Integer = 0
                                        If Not IsDBNull(oRow("IDShift2")) Then intIDShift2 = oRow("IDShift2")
                                        If Not IsDBNull(oRow("IDShift3")) Then intIDShift3 = oRow("IDShift3")
                                        If Not IsDBNull(oRow("IDShift4")) Then intIDShift4 = oRow("IDShift4")
                                        Dim xStartShift1 As DateTime = Nothing
                                        Dim xStartShift2 As DateTime = Nothing
                                        Dim xStartShift3 As DateTime = Nothing
                                        Dim xStartShift4 As DateTime = Nothing
                                        If Not IsDBNull(oRow("StartShift1")) Then xStartShift1 = oRow("StartShift1")
                                        If Not IsDBNull(oRow("StartShift2")) Then xStartShift2 = oRow("StartShift2")
                                        If Not IsDBNull(oRow("StartShift3")) Then xStartShift3 = oRow("StartShift3")
                                        If Not IsDBNull(oRow("StartShift4")) Then xStartShift4 = oRow("StartShift4")

                                        If intIDShift1 <> 0 And intIDShift2 <> 0 And intIDShift3 <> 0 And intIDShift4 <> 0 Then
                                            ' Todos los horarios alternativos están informados
                                            oState.Result = EmployeeResultEnum.ShiftFullAlterShift
                                        ElseIf (intIDShift1 = intIDAlterShift And xStartShift1 = xAlterStartShift) Or (intIDShift2 = intIDAlterShift And xStartShift2 = xAlterStartShift) Or (intIDShift3 = intIDAlterShift And xStartShift3 = xAlterStartShift) Or (intIDShift4 = intIDAlterShift And xStartShift4 = xAlterStartShift) Then
                                            ' El horario ya está asignado
                                            oState.Result = EmployeeResultEnum.ShiftAlreadyAssigned
                                        Else
                                            Dim intIDShiftIndex As Integer = 0
                                            If intIDShift2 = 0 Then
                                                intIDShiftIndex = 2
                                            ElseIf intIDShift3 = 0 Then
                                                intIDShiftIndex = 3
                                            ElseIf intIDShift4 = 0 Then
                                                intIDShiftIndex = 4
                                            End If
                                            If intIDShiftIndex > 0 Then
                                                oRow("IDShift" & intIDShiftIndex.ToString) = intIDAlterShift
                                                If xAlterStartShift <> Nothing Then
                                                    oRow("StartShift" & intIDShiftIndex.ToString) = xAlterStartShift
                                                Else
                                                    oRow("StartShift" & intIDShiftIndex.ToString) = DBNull.Value
                                                End If
                                            End If
                                        End If

                                        If oRow.RowState = DataRowState.Modified Then

                                            oRow("Status") = 0
                                            oRow("GUID") = ""
                                            ad.Update(tb)

                                            ' Notificamos el cambio
                                            roConnector.InitTask(TasksType.DAILYSCHEDULE)

                                        End If
                                    Else ' No hay el horario principal informado
                                        oState.Result = EmployeeResultEnum.ShiftInvalidPrimaryShift
                                    End If

                                End If

                                bolRet = (oState.Result = EmployeeResultEnum.NoError)
                            Else ' No hay el horario principal informado
                                oState.Result = EmployeeResultEnum.ShiftInvalidPrimaryShift
                            End If
                        Catch ex As DbException
                            oState.UpdateStateInfo(ex, "roEmployees::AssignAlterShift")
                        Catch ex As Exception
                            oState.UpdateStateInfo(ex, "roEmployees::AssignAlterShift")
                        End Try
                    Else
                        oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                    End If
                Else
                    oState.Result = EmployeeResultEnum.EmployeeNoContract
                End If
            Else
                oState.Result = EmployeeResultEnum.AccessDenied
            End If

            Return bolRet

        End Function

        Public Shared Function AssignWeekShifts(ByVal lstEmployees As ArrayList, ByVal lstWeekShifts As ArrayList, ByVal lstWeekStartShifts As Generic.List(Of DateTime),
                                         ByVal lstWeekAssignments As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date,
                                         ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                         ByRef intIDEmployeeLocked As Integer, ByRef xDateLocked As Date, ByRef oState As Employee.roEmployeeState, Optional ByVal keepHolidays As Boolean = True,
                                         Optional ByVal bolAudit As Boolean = True, Optional ByVal oTask As roLiveTask = Nothing) As Boolean

            Dim bolRet As Boolean = False
            Dim bolNotify As Boolean = False
            Dim bolTemp As Boolean = False

            Try

                Dim oLicense As New roServerLicense
                Dim obolHRSchedulingInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim totalActions As Integer = lstEmployees.Count * xEndDate.Subtract(xBeginDate).TotalDays
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Integer = 100 / totalActions

                If xBeginDate <= xEndDate Then

                    If lstWeekShifts.Count = 7 Then

                        Dim bolExistBusinessGroup As Boolean = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from ShiftGroups WHERE isnull(BusinessGroup, '') <> ''")) > 0)

                        If obolHRSchedulingInstalled Then
                            obolHRSchedulingInstalled = (Any2Double(ExecuteScalar("@SELECT# count(*) as Total from Assignments ")) > 0)
                        End If

                        For Each intIDEmployee As Integer In lstEmployees

                            If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 AndAlso intIDEmployeeLocked = intIDEmployee) Then

                                Dim xFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(intIDEmployee, False, oState)

                                intIDEmployeeLocked = 0 ' Inicializamos id último empleado bloqueado.

                                Dim xDate As Date = xBeginDate
                                If xDateLocked <> Nothing Then
                                    xDate = xDateLocked
                                    xDateLocked = Nothing ' Inicializamos la fecha del último bloqueo
                                End If

                                Dim intIDShift As Integer
                                Dim xStartShift As DateTime
                                Dim intIDAssignment As Integer
                                While xDate <= xEndDate And oState.Result <> EmployeeResultEnum.Exception

                                    If xFreezeDate = Nothing OrElse xFreezeDate < xDate Then ' Verificamos que no estemos en periodo de congelación

                                        Select Case xDate.DayOfWeek
                                            Case DayOfWeek.Monday : intIDShift = lstWeekShifts(0) : xStartShift = lstWeekStartShifts(0) : intIDAssignment = lstWeekAssignments(0)
                                            Case DayOfWeek.Tuesday : intIDShift = lstWeekShifts(1) : xStartShift = lstWeekStartShifts(1) : intIDAssignment = lstWeekAssignments(1)
                                            Case DayOfWeek.Wednesday : intIDShift = lstWeekShifts(2) : xStartShift = lstWeekStartShifts(2) : intIDAssignment = lstWeekAssignments(2)
                                            Case DayOfWeek.Thursday : intIDShift = lstWeekShifts(3) : xStartShift = lstWeekStartShifts(3) : intIDAssignment = lstWeekAssignments(3)
                                            Case DayOfWeek.Friday : intIDShift = lstWeekShifts(4) : xStartShift = lstWeekStartShifts(4) : intIDAssignment = lstWeekAssignments(4)
                                            Case DayOfWeek.Saturday : intIDShift = lstWeekShifts(5) : xStartShift = lstWeekStartShifts(5) : intIDAssignment = lstWeekAssignments(5)
                                            Case DayOfWeek.Sunday : intIDShift = lstWeekShifts(6) : xStartShift = lstWeekStartShifts(6) : intIDAssignment = lstWeekAssignments(6)
                                        End Select

                                        If keepHolidays = False Then
                                            roScheduler.AssignShiftEx(intIDEmployee, xDate, -1, 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, 0,
                                                                     _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezeDate, bolExistBusinessGroup, True, , bolAudit)
                                        End If

                                        bolTemp = roScheduler.AssignShiftEx(intIDEmployee, xDate, intIDShift, 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, intIDAssignment,
                                                                   _LockedDayAction, _CoverageDayAction, oState, obolHRSchedulingInstalled, xFreezeDate, bolExistBusinessGroup, True, , bolAudit, _ShiftPermissionAction)

                                        If _ShiftPermissionAction = ShiftPermissionAction.ContinueAndStop Then
                                            _ShiftPermissionAction = ShiftPermissionAction.None
                                        End If

                                        If bolTemp Then
                                            bolNotify = True
                                        End If

                                        If oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                           oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                           oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                            ' El día está bloqueado o hay covertura. Obtenemos la fecha y el id del empleado bloqueado.
                                            xDateLocked = xDate
                                            intIDEmployeeLocked = intIDEmployee
                                        End If

                                    End If

                                    xDate = xDate.AddDays(1)

                                    If oState.Result = EmployeeResultEnum.Exception Or
                                       oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                       oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                       oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then Exit While

                                    If oTask IsNot Nothing Then
                                        oTask.Progress = oTask.Progress + stepProgress
                                        oTask.Save(Nothing)
                                    End If

                                End While

                                If oState.Result = EmployeeResultEnum.Exception Or
                                   oState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                   oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                                   oState.Result = EmployeeResultEnum.ShiftWithoutPermission Then Exit For
                            End If

                        Next

                        If bolNotify Then
                            ' Notificamos el cambio
                            roConnector.InitTask(TasksType.DAILYSCHEDULE)
                        End If

                        bolRet = (oState.Result = EmployeeResultEnum.NoError Or
                                  oState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                                  oState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                                  oState.Result = EmployeeResultEnum.AccessDenied)
                    Else
                        oState.Result = EmployeeResultEnum.InvalidWeekShiftsList
                    End If
                Else
                    oState.Result = EmployeeResultEnum.InvalidDateInterval
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::AssignWeekShifts")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::AssignWeekShifts")
            End Try

            Return bolRet

        End Function

        Public Shared Function RemoveShift(ByVal intShiftPosition As Integer, ByVal intIDEmployee As Long, ByVal xShiftDate As Date, ByRef _LockedDayAction As LockedDayAction, ByRef _CoverageDayAction As LockedDayAction,
                                           ByRef oState As Employee.roEmployeeState, Optional ByVal bolCheckPermission As Boolean = True, Optional ByVal bolNotify As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bolHasPermission As Boolean = True
            If bolCheckPermission Then
                ' Verificamos los permisos del pasaporte actual sobre la planificación
                bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, "Calendar.Scheduler", Permission.Write, intIDEmployee)

            End If

            If bolHasPermission Then

                ' ****
                'If Any2Time(ShiftDate).NumericValue > Any2Time(GetFirstDate(ServerHandle)).NumericValue Then
                ' ****

                If Not roBusinessSupport.InPeriodOfFreezing(xShiftDate, intIDEmployee, oState) Then

                    Try

                        Dim strSQL As String

                        strSQL = "@SELECT# * FROM DailySchedule " &
                             "WHERE IDEmployee = " & intIDEmployee & " AND " &
                                   "Date = " & Any2Time(xShiftDate).SQLSmallDateTime
                        Dim tb As New DataTable
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim ad As DbDataAdapter = CreateDataAdapter(cmd, True)
                        ad.Fill(tb)

                        If tb.Rows.Count = 1 Then

                            Dim oRow As DataRow = tb.Rows(0)

                            Dim bolRemove As Boolean = True

                            If Any2Boolean(oRow("LockedDay")) Then
                                Select Case _LockedDayAction
                                    ' El día a borrar está bloqueado
                                    Case LockedDayAction.None
                                        ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se borre
                                        oState.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                        bolRemove = False
                                    Case LockedDayAction.ReplaceFirst
                                        ' Se tiene que borrar igualmente la planificación del día
                                        bolRemove = True
                                        _LockedDayAction = LockedDayAction.None
                                    Case LockedDayAction.ReplaceAll
                                        ' Se tiene que borrar igualmente la planificación del día
                                        bolRemove = True
                                    Case LockedDayAction.NoReplaceFirst
                                        ' No se tiene que borrar la planificación del día
                                        bolRemove = False
                                        _LockedDayAction = LockedDayAction.None
                                    Case LockedDayAction.NoReplaceAll
                                        ' No se tiene que borrar la planificación del día
                                        bolRemove = False
                                End Select
                            End If

                            If bolRemove Then

                                If Any2Boolean(oRow("IsCovered")) OrElse Any2Integer(oRow("OldIDAssignment")) > 0 Then

                                    Select Case _CoverageDayAction
                                        Case LockedDayAction.None
                                            ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se borre
                                            oState.Result = EmployeeResultEnum.DailyScheduleCoverageDay
                                            bolRemove = False
                                        Case LockedDayAction.ReplaceFirst
                                            ' Se tiene que borrar igualmente
                                            bolRemove = True
                                            _CoverageDayAction = LockedDayAction.None
                                        Case LockedDayAction.ReplaceAll
                                            ' Se tiene que borrar igualmente
                                            bolRemove = True
                                        Case LockedDayAction.NoReplaceFirst
                                            ' No se tiene que asignar
                                            bolRemove = False
                                            _CoverageDayAction = LockedDayAction.None
                                        Case LockedDayAction.NoReplaceAll
                                            ' No se tiene que borrar
                                            bolRemove = False
                                    End Select

                                End If

                            End If

                            If bolRemove Then

                                oRow("Status") = 0
                                oRow("GUID") = ""
                                If intShiftPosition = -1 Then ' Borrar todos los horarios de la fecha y empleado
                                    oRow("IDShift1") = DBNull.Value
                                    oRow("IDShift2") = DBNull.Value
                                    oRow("IDShift3") = DBNull.Value
                                    oRow("IDShift4") = DBNull.Value
                                    oRow("IDShiftBase") = DBNull.Value
                                    oRow("IDPreviousShift") = DBNull.Value
                                    oRow("StartShift1") = DBNull.Value
                                    oRow("StartShift2") = DBNull.Value
                                    oRow("StartShift3") = DBNull.Value
                                    oRow("StartShift4") = DBNull.Value
                                    oRow("StartShiftBase") = DBNull.Value
                                    oRow("IDAssignment") = DBNull.Value
                                    oRow("IDAssignmentBase") = DBNull.Value
                                    oRow("IsCovered") = DBNull.Value
                                    oRow("IsHolidays") = DBNull.Value
                                    oRow("OldIDAssignment") = DBNull.Value
                                    oRow("IDEmployeeCovered") = DBNull.Value
                                    oRow("OldIDShift") = DBNull.Value
                                    oRow("LayersDefinition") = DBNull.Value
                                    oRow("ExpectedWorkingHours") = DBNull.Value
                                Else
                                    oRow("IDShift" & intShiftPosition.ToString) = DBNull.Value
                                    oRow("StartShift" & intShiftPosition.ToString) = DBNull.Value
                                    If intShiftPosition = 1 Then
                                        oRow("IDAssignment") = DBNull.Value
                                        oRow("IsCovered") = DBNull.Value
                                        oRow("OldIDAssignment") = DBNull.Value
                                        oRow("IDEmployeeCovered") = DBNull.Value
                                        oRow("OldIDShift") = DBNull.Value
                                    End If
                                End If

                                ad.Update(tb)

                            End If

                            bolRet = (oState.Result = EmployeeResultEnum.NoError)

                            If bolNotify AndAlso bolRemove Then
                                ' Notificamos el cambio
                                roConnector.InitTask(TasksType.DAILYSCHEDULE)

                                ' Marcamos las dotaciones para el empleado y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                Dim oSchedulerState As New Scheduler.roSchedulerState
                                roBusinessState.CopyTo(oState, oSchedulerState)
                                bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, intIDEmployee, , xShiftDate, bolNotify)
                                roBusinessState.CopyTo(oSchedulerState, oState)

                            End If

                        End If
                    Catch ex As DbException
                        oState.UpdateStateInfo(ex, "roEmployees::RemoveShift")
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roEmployees::RemoveShift")
                    End Try
                Else
                    oState.Result = EmployeeResultEnum.InPeriodOfFreezing
                End If
            Else
                oState.Result = EmployeeResultEnum.AccessDenied
            End If

            Return bolRet
        End Function

        Public Shared Function GetLatestPlanDate(ByVal intIDEmployee As Integer, ByRef oState As Employee.roEmployeeState) As roTime

            Dim xPlanDate As roTime = Nothing

            oState.UpdateStateInfo()

            Try
                Dim strSQL As String
                strSQL = "@SELECT# MAX(Date) From DailySchedule Where IDEmployee = " & intIDEmployee
                Dim oDate As Object = ExecuteScalar(strSQL)
                If oDate IsNot Nothing Then
                    xPlanDate = Any2Time(oDate)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetLatestPlanDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetLatestPlanDate")
            End Try

            Return xPlanDate

        End Function

#End Region

#Region "Presence query methods"

        Public Shared Function PresenceDetailEx(ByVal intIDEmployee As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByRef oState As Employee.roEmployeeState,
                                       Optional ByVal IdGroup As Integer = False) As DataTable

            oState.UpdateStateInfo()

            Dim dsPresenceDetail As New PresenceDetailDataSet()
            Dim tb As DataTable = dsPresenceDetail.Tables("PresenceDetail")

            Dim tbMoves As DataTable = Nothing
            Try

                Dim oRow As DataRow
                Dim strSQL As String

                strSQL = "@SELECT# ShiftDate, DateTime, ActualType FROM Punches "
                If IdGroup <> -1 Then
                    strSQL &= "INNER JOIN EmployeeGroups ON Punches.IDEmployee = EmployeeGroups.IDEmployee AND Punches.ShiftDate >= EmployeeGroups.BeginDate AND Punches.ShiftDate <= EmployeeGroups.EndDate "
                End If

                strSQL &= " INNER Join sysrovwSecurity_PermissionOverEmployeeAndFeature poe with (NOLOCK) on poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = Punches.IDEmployee And DateAdd(Day, DateDiff(Day, 0, ShiftDate), 0) between poe.BeginDate And poe.EndDate and poe.idFeature=2 and poe.FeaturePermission > 3 "

                strSQL &= " WHERE Punches.IDEmployee = " & intIDEmployee & " AND " &
                         "Punches.ShiftDate BETWEEN " & roTypes.Any2Time(xBeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(xEndDate).SQLSmallDateTime & " AND " &
                         "Punches.ActualType IN(" & PunchTypeEnum._OUT & "," & PunchTypeEnum._IN & ") "
                If IdGroup <> -1 Then
                    strSQL &= " AND EmployeeGroups.IdGroup = " & IdGroup
                End If

                strSQL &= " ORDER BY ShiftDate, DateTime, ID"

                tbMoves = CreateDataTable(strSQL, )

                Dim xDate As Nullable(Of DateTime) = Nothing
                Dim strMoves As String = ""
                Dim strMoveIn As String
                Dim strMoveOut As String
                Dim intMovesPairs As Integer = 0
                Dim n As Integer = 1

                If tbMoves IsNot Nothing AndAlso tbMoves.Rows.Count > 0 Then
                    For Each dtRow As DataRow In tbMoves.Rows
                        If xDate.HasValue AndAlso xDate.Value <> dtRow("ShiftDate") Then
                            If strMoves <> "" Then
                                If strMoves.Substring(strMoves.Length - 2, 2) = vbCrLf Then
                                    strMoves = strMoves.Substring(0, strMoves.Length - 2)
                                End If
                            End If
                            oRow = tb.NewRow
                            oRow("Date") = xDate.Value
                            oRow("Moves") = strMoves
                            oRow("PresenceMinutes") = roScheduler.PresenceMinutes(intIDEmployee, xDate.Value, oState)
                            oRow("MovesPairs") = intMovesPairs
                            tb.Rows.Add(oRow)
                            strMoves = ""
                            intMovesPairs = 0
                            n = 1
                        End If

                        xDate = dtRow("ShiftDate")

                        If dtRow("ActualType") = PunchTypeEnum._IN Then
                            strMoveIn = Format(dtRow("DateTime"), "HH:mm") & " E"
                        Else
                            strMoveIn = ""
                        End If
                        If dtRow("ActualType") = PunchTypeEnum._OUT Then
                            strMoveOut = Format(dtRow("DateTime"), "HH:mm") & " S"
                        Else
                            strMoveOut = ""
                        End If
                        If strMoveIn <> "" Or strMoveOut <> "" Then
                            If strMoveIn <> "" Then
                                strMoves &= strMoveIn
                            Else
                                strMoves &= strMoveOut
                            End If
                            If n <> 2 Then
                                strMoves &= ","
                            End If

                            If n = 2 Then
                                strMoves &= vbCrLf
                                n = 0
                            End If
                            n += 1
                        End If
                    Next
                End If

                If strMoves <> "" Then
                    If strMoves.Substring(strMoves.Length - 1, 1) = "," Then
                        strMoves = strMoves.Substring(0, strMoves.Length - 1)
                    End If
                    If strMoves.Substring(strMoves.Length - 2, 2) = vbCrLf Then
                        strMoves = strMoves.Substring(0, strMoves.Length - 2)
                    End If
                    oRow = tb.NewRow
                    oRow("Date") = xDate.Value
                    oRow("Moves") = strMoves
                    oRow("PresenceMinutes") = roScheduler.PresenceMinutes(intIDEmployee, xDate.Value, oState)
                    oRow("MovesPairs") = intMovesPairs
                    tb.Rows.Add(oRow)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::PresenceDetail")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::PresenceDetail")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function PresenceMinutes(ByVal intIDEmployee As Integer, ByVal xDate As DateTime, ByRef oState As Employee.roEmployeeState) As Integer

            Dim intRet As Integer = 0

            oState.UpdateStateInfo()

            Dim xInDateTime As DateTime
            Dim xOutDateTime As DateTime
            Dim bolIn As Boolean = False
            Dim bolOut As Boolean = False

            Try

                '## Dim strSQL As String = "@SELECT# * " & _
                '##                       "FROM Moves " & _
                '##                       "WHERE IDEmployee = " & intIDEmployee.ToString & " AND " & _
                '##                             "ShiftDate = " & roTypes.Any2Time(xDate).SQLSmallDateTime & " " & _
                '##                       "ORDER BY [ID]"

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM Punches " &
                                       "WHERE IDEmployee = " & intIDEmployee.ToString & " AND " &
                                             "ShiftDate = " & roTypes.Any2Time(roTypes.Any2Time(xDate).DateOnly).SQLSmallDateTime & " " &
                                             " AND ActualType IN(" & Any2Double(PunchTypeEnum._IN) & "," & Any2Double(PunchTypeEnum._OUT) & ") " &
                                       "ORDER BY [DateTime], [ID]"

                Dim dt As DataTable = CreateDataTable(strSQL, )

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each oRow As DataRow In dt.Rows
                        '## If Not IsDBNull(rd("InDateTime")) And Not IsDBNull(rd("OutDateTime")) Then
                        '##    intRet += DateDiff(DateInterval.Minute, rd("InDateTime"), rd("OutDateTime"))
                        '## End If

                        ' Si es una entrada
                        If Not IsDBNull(oRow("DateTime")) And oRow("ActualType") = PunchTypeEnum._IN Then
                            xInDateTime = oRow("DateTime")
                            bolIn = True
                        End If

                        ' Si es una Salida
                        If Not IsDBNull(oRow("DateTime")) And oRow("ActualType") = PunchTypeEnum._OUT Then
                            xOutDateTime = oRow("DateTime")
                            bolOut = True
                        End If

                        ' Si tenemos entrada y salida acumulados
                        If bolIn And bolOut Then
                            'intRet += DateDiff(DateInterval.Minute, xInDateTime, xOutDateTime)
                            'Quitamos segundos para que coincida con saldos
                            intRet += DateDiff(DateInterval.Minute, xInDateTime.AddSeconds(-1 * xInDateTime.Second), xOutDateTime.AddSeconds(-1 * xOutDateTime.Second))
                            bolIn = False
                            bolOut = False
                        End If

                        ' Si solo tenemos salida es que esta desemparejada
                        If Not bolIn And bolOut Then
                            bolOut = False
                        End If
                    Next
                End If

                'Dim rd As DbDataReader = Nothing
                'rd = CreateDataReader(strSQL)
                'While rd.Read
                '    '## If Not IsDBNull(rd("InDateTime")) And Not IsDBNull(rd("OutDateTime")) Then
                '    '##    intRet += DateDiff(DateInterval.Minute, rd("InDateTime"), rd("OutDateTime"))
                '    '## End If

                '    ' Si es una entrada
                '    If Not IsDBNull(rd("DateTime")) And rd("ActualType") = Punch.roPunch.PunchTypeEnum._IN Then
                '        xInDateTime = rd("DateTime")
                '        bolIn = True
                '    End If

                '    ' Si es una Salida
                '    If Not IsDBNull(rd("DateTime")) And rd("ActualType") = Punch.roPunch.PunchTypeEnum._OUT Then
                '        xOutDateTime = rd("DateTime")
                '        bolOut = True
                '    End If

                '    ' Si tenemos entrada y salida acumulados
                '    If bolIn And bolOut Then
                '        intRet += DateDiff(DateInterval.Minute, xInDateTime, xOutDateTime)
                '        bolIn = False
                '        bolOut = False
                '    End If

                '    ' Si solo tenemos salida es que esta desemparejada
                '    If Not bolIn And bolOut Then
                '        bolOut = False
                '    End If
                'End While
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::PresenceMinutes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::PresenceMinutes")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function PresenceDetail(ByVal intIDEmployee As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByRef oState As Employee.roEmployeeState) As DataTable

            oState.UpdateStateInfo()

            Dim dsPresenceDetail As New PresenceDetailDataSet()
            Dim tb As DataTable = dsPresenceDetail.Tables("PresenceDetail")

            Dim tbDetail As DataTable = Nothing
            Try

                Dim oRow As DataRow

                Dim strSQL As String = "@SELECT# DailySchedule.[Date], Punches.DateTime, Punches.ActualType " &
                                       "FROM DailySchedule LEFT JOIN Punches " &
                                                "ON DailySchedule.[Date] = Punches.ShiftDate AND " &
                                                   "DailySchedule.IDEmployee = Punches.IDEmployee " &
                                       "WHERE DailySchedule.IDEmployee = " & intIDEmployee.ToString & " AND " &
                                             "DailySchedule.[Date] between " & roTypes.Any2Time(xBeginDate).SQLSmallDateTime & " AND " &
                                                                               roTypes.Any2Time(xEndDate).SQLSmallDateTime & " AND " &
                                                                               " Punches.ActualType IN(" & PunchTypeEnum._OUT & "," & PunchTypeEnum._IN & ")" & " " &
                                       "ORDER BY DailySchedule.[Date], Punches.DateTime, Punches.ID"

                tbDetail = CreateDataTable(strSQL, )

                Dim xDate As Nullable(Of DateTime) = Nothing
                Dim strMoves As String = ""
                Dim strMoveIn As String
                Dim strMoveOut As String
                Dim intMovesPairs As Integer = 0
                Dim n As Integer = 1
                If tbDetail IsNot Nothing AndAlso tbDetail.Rows.Count > 0 Then
                    For Each oDetailRow As DataRow In tbDetail.Rows
                        If xDate.HasValue AndAlso xDate.Value <> oDetailRow("Date") Then
                            If strMoves <> "" Then
                                If strMoves.Substring(strMoves.Length - 2, 2) = vbCrLf Then
                                    strMoves = strMoves.Substring(0, strMoves.Length - 2)
                                End If
                            End If
                            oRow = tb.NewRow
                            oRow("Date") = xDate.Value
                            oRow("Moves") = strMoves
                            oRow("PresenceMinutes") = roScheduler.PresenceMinutes(intIDEmployee, xDate.Value, oState)
                            oRow("MovesPairs") = intMovesPairs
                            tb.Rows.Add(oRow)
                            strMoves = ""
                            intMovesPairs = 0
                            n = 1
                        End If

                        xDate = oDetailRow("Date")

                        If oDetailRow("ActualType") = PunchTypeEnum._IN Then
                            strMoveIn = Format(oDetailRow("DateTime"), "HH:mm") & " E"
                        Else
                            strMoveIn = ""
                        End If
                        If oDetailRow("ActualType") = PunchTypeEnum._OUT Then
                            strMoveOut = Format(oDetailRow("DateTime"), "HH:mm") & " S"
                        Else
                            strMoveOut = ""
                        End If
                        If strMoveIn <> "" Or strMoveOut <> "" Then
                            If strMoveIn <> "" Then
                                strMoves &= strMoveIn
                            Else
                                strMoves &= strMoveOut
                            End If
                            If n <> 2 Then
                                strMoves &= ","
                            End If

                            If n = 2 Then
                                strMoves &= vbCrLf
                                n = 0
                            End If
                            n += 1
                        End If
                    Next
                End If

                If strMoves <> "" Then
                    If strMoves.Substring(strMoves.Length - 1, 1) = "," Then
                        strMoves = strMoves.Substring(0, strMoves.Length - 1)
                    End If
                    If strMoves.Substring(strMoves.Length - 2, 2) = vbCrLf Then
                        strMoves = strMoves.Substring(0, strMoves.Length - 2)
                    End If
                    oRow = tb.NewRow
                    oRow("Date") = xDate.Value
                    oRow("Moves") = strMoves
                    oRow("PresenceMinutes") = roScheduler.PresenceMinutes(intIDEmployee, xDate.Value, oState)
                    oRow("MovesPairs") = intMovesPairs
                    tb.Rows.Add(oRow)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::PresenceDetail")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::PresenceDetail")
            Finally

            End Try

            Return tb

        End Function



        Public Shared Function GetPresenceStatusEx(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByRef _LastPunchType As PunchStatus, ByRef _LastPunchDateTime As DateTime, ByRef _LastPunch As Punch.roPunch, ByRef _PresenceMinutes As Integer, ByVal _State As Employee.roEmployeeState) As PresenceStatus
            '
            ' Devuelve el estado del empleado
            '

            Dim oRet As PresenceStatus = PresenceStatus.Outside

            Try

                Dim lngLastPunchID As Long

                ' Obtener información del último marcaje de presencia del empleado
                Dim oPunchState As New Punch.roPunchState : roBusinessState.CopyTo(_State, oPunchState)
                _LastPunch = New Punch.roPunch(_IDEmployee, -1, oPunchState)
                _LastPunch.GetLastPunchPres(_LastPunchType, _LastPunchDateTime, lngLastPunchID)
                _LastPunch.ID = lngLastPunchID
                _LastPunch.Load()
                roBusinessState.CopyTo(oPunchState, _State)

                If _LastPunchType <> MovementStatus.Indet_ Then

                    ' Verificar tiempo entre marcajes (MaxMovementHours)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim intMaxMovement As Integer = 0
                    Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                    If oTime.IsValid Then intMaxMovement = oTime.Minutes

                    If intMaxMovement > 0 AndAlso
                       DateDiff(DateInterval.Minute, _LastPunchDateTime, _InputDateTime) > intMaxMovement Then

                        ' El tiempo entre el marcaje actual y el anterior és superior al máximo (MaxMovementHours)
                        oRet = PresenceStatus.Outside
                    Else

                        If _LastPunchType = PunchStatus.In_ Then
                            oRet = PresenceStatus.Inside
                        ElseIf _LastPunchType = PunchStatus.Out_ Then
                            oRet = PresenceStatus.Outside
                        ElseIf _LastPunchType = PunchStatus.Indet_ Then
                            oRet = PresenceStatus.Outside
                        End If

                    End If
                Else
                    oRet = PresenceStatus.Outside
                End If

                ' Obtenemos los minutos de presencia del día actual
                _PresenceMinutes = roScheduler.PresenceMinutes(_IDEmployee, _InputDateTime, _State)

                If oRet = PresenceStatus.Inside Then
                    If _LastPunch.DateTime.HasValue Then
                        _PresenceMinutes += DateDiff(DateInterval.Minute, _LastPunch.DateTime.Value, _InputDateTime)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::GetPresenceStatueEx")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::GetPresenceStatusEx")
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Obtiene el estado de presencia de un empleado en una fecha/hora indicada.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora en la que se obtiene el estado</param>
        ''' <param name="_LastMoveType">Devuelve el tipo del último movimiento de presencia del empleado</param>
        ''' <param name="_LastMoveDateTime">Devuelve la fecha y hora del último movimiento del empleado</param>
        ''' <param name="_LastMove">Devuelve el último movimiento de presencia del empleado</param>
        ''' <param name="_PresenceMinutes">Tiempo de presencia de la fecha actual (en minutos)</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPresenceStatus(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByRef _LastMoveType As MovementStatus, ByRef _LastMoveDateTime As DateTime, ByRef _LastMove As Move.roMove, ByRef _PresenceMinutes As Integer, ByVal _State As Employee.roEmployeeState) As PresenceStatus

            Dim oRet As PresenceStatus = PresenceStatus.Outside

            Try

                Dim lngLastMoveID As Long

                ' Obtener información del último marcaje de presencia del empleado
                Dim oMoveState As New Move.roMoveState : roBusinessState.CopyTo(_State, oMoveState)
                _LastMove = New Move.roMove(_IDEmployee, -1, oMoveState)
                _LastMove.GetLastMove(_LastMoveType, _LastMoveDateTime, lngLastMoveID)
                _LastMove.IDMove = lngLastMoveID
                _LastMove.Load()
                roBusinessState.CopyTo(oMoveState, _State)

                If _LastMoveType <> MovementStatus.Indet_ Then

                    ' Verificar tiempo entre marcajes (MaxMovementHours)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim intMaxMovement As Integer = 0
                    Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                    If oTime.IsValid Then intMaxMovement = oTime.Minutes

                    If intMaxMovement > 0 AndAlso
                       DateDiff(DateInterval.Minute, _LastMoveDateTime, _InputDateTime) > intMaxMovement Then

                        ' El tiempo entre el marcaje actual y el anterior és superior al máximo (MaxMovementHours)
                        oRet = PresenceStatus.Outside
                    Else

                        If _LastMoveType = MovementStatus.In_ Then
                            oRet = PresenceStatus.Inside
                        ElseIf _LastMoveType = MovementStatus.Out_ Then
                            oRet = PresenceStatus.Outside
                        ElseIf _LastMoveType = MovementStatus.Indet_ Then
                            oRet = PresenceStatus.Outside
                        End If

                    End If
                Else
                    oRet = PresenceStatus.Outside
                End If

                ' Obtenemos los minutos de presencia del día actual
                roScheduler.PresenceMinutes(_IDEmployee, _InputDateTime, _State)

                If oRet = PresenceStatus.Inside Then
                    If _LastMove.DateTimeIN.HasValue Then
                        _PresenceMinutes += DateDiff(DateInterval.Minute, _LastMove.DateTimeIN.Value, _InputDateTime)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::GetPresenceStatue")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::GetPresenceStatus")
            End Try
            Return oRet

        End Function

#End Region

#Region "Coverages"

        ''' <summary>
        ''' Devuelve la lista de posibles empleados que pueden realizar una cobertura del empleado para una fecha en concreto.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado al que se quiere cubrir</param>
        ''' <param name="_CoverageDate">Fecha de la cobertura</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>Devuelve un datatable con las siguientes columnas: IDEmployee, EmployeeName, IDShift, ShiftName, IDAssignment, AssignmentName, IDGroup, GroupName, Points, Cost, Suitability, Coverage. </returns>
        ''' <remarks></remarks>
        Public Shared Function GetCoverageEmployees(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByVal _State As Employee.roEmployeeState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String

                Dim intIDGroup As Integer = -1
                Dim intIDAssignment As Integer = -1

                ' Obtenemos la dotación a la que pertenece el puesto que está asignado al empleado para determinar el grupo al que pertenece
                strSQL = "@DECLARE# @Path nvarchar(4000) " &
                         "@SELECT# @Path = sysrovwAllEmployeeGroups.Path " &
                         "FROM sysrovwAllEmployeeGroups " &
                         "WHERE sysrovwAllEmployeeGroups.CurrentEmployee = 1 AND sysrovwAllEmployeeGroups.IDEmployee = " & _IDEmployee.ToString & " AND " &
                               "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(_CoverageDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(_CoverageDate).SQLSmallDateTime & " " &
                         "@SELECT# DailyCoverage.IDGroup " &
                         "FROM DailyCoverage INNER JOIN DailySchedule " &
                                    "ON DailyCoverage.IDAssignment = DailySchedule.IDAssignment AND DailyCoverage.Date = DailySchedule.Date " &
                                    "INNER JOIN Groups ON Groups.ID = DailyCoverage.IDGroup	" &
                         "WHERE DailySchedule.IDEmployee = " & _IDEmployee.ToString & " AND DailySchedule.Date = " & Any2Time(_CoverageDate).SQLSmallDateTime & " AND " &
                               "DailyCoverage.IDGroup IN (@SELECT# * FROM SplitInt(@Path, '\')) " &
                         "ORDER BY Groups.Path DESC"
                Dim tbDailyCoverage As DataTable = CreateDataTable(strSQL, )
                If tbDailyCoverage IsNot Nothing AndAlso tbDailyCoverage.Rows.Count > 0 Then
                    intIDGroup = Any2Integer(tbDailyCoverage.Rows(0).Item("IDGroup"))
                End If

                ' Obtenemos el puesto a cubrir
                strSQL = "@SELECT# IDAssignment FROM DailySchedule WHERE IDEmployee = " & _IDEmployee.ToString & " AND Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                Dim tbDailySchedule As DataTable = CreateDataTable(strSQL, )
                If tbDailySchedule IsNot Nothing AndAlso tbDailySchedule.Rows.Count = 1 Then
                    intIDAssignment = Any2Integer(tbDailySchedule.Rows(0).Item("IDAssignment"))
                End If

                ' Empleados sin horario asignado y que puedan cubrir el puesto
                strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                "NULL AS 'IDAssignment', '' AS 'AssignmentName', " &
                                "NULL AS 'IDShift1', '' AS 'ShiftName', " &
                                "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                "(@SELECT# EmployeeAssignments.Suitability FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment = 2) AS  'Suitability', NULL AS 'Coverage', 0.000 AS 'Cost', NULL AS 'Points'," &
                                "40000 AS 'OrderField', '' as ConceptValue " &
                         "FROM sysrovwAllEmployeeGroups " &
                         "WHERE (sysrovwAllEmployeeGroups.CurrentEmployee = 1) AND (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                               "sysrovwAllEmployeeGroups.IDEmployee <> @IDEmployee AND " &
                               "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                               "ISNULL((@SELECT# COUNT(*) FROM DailySchedule WHERE DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND DailySchedule.Date = @CoverageDate AND ISNULL(DailySchedule.IDShift1, 0) > 0), 0) = 0 AND " &
                               "(@SELECT# COUNT(*) FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment = @IDAssignment) = 1 "
                strSQL &= "UNION "

                ' Empleados con horario asignado compatible con el puesto, sin puesto asignado y que puedan cubrir el puesto
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                 "NULL AS 'IDAssignment', '' AS 'AssignmentName', " &
                                 "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                 "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'Suitability', " &
                                 "ISNULL(ShiftAssignments.Coverage,0) * 100 AS 'Coverage', " &
                                 "0.000 AS 'Cost', " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'Points', " &
                                 "20000 + (ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100)) AS 'OrderField' , '' as ConceptValue " &
                          "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                    "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN ShiftAssignments ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                    "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                                    "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                          "WHERE (sysrovwAllEmployeeGroups.CurrentEmployee = 1) AND (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                                "sysrovwAllEmployeeGroups.IDEmployee <> @IDEmployee AND " &
                                "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                                "DailySchedule.Date = @CoverageDate AND ShiftAssignments.IDAssignment = @IDAssignment AND ISNULL(DailySchedule.IDAssignment,0) = 0 AND	" &
                                "EmployeeAssignments.IDAssignment = @IDAssignment "
                strSQL &= "UNION "

                ' Empleados con horario asignado compatible con el puesto, con puesto asignado distinto al puesto a cubrir (primero los que el puesto asignadono intervenga en ninguna de las dotaciones) y que puedan cubrir el puesto.
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                 "DailySchedule.IDAssignment AS 'IDAssignment', Assignments.Name AS 'AssignmentName', " &
                                 "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                 "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'Suitability', " &
                                 "ISNULL(ShiftAssignments.Coverage,0) * 100 AS 'Coverage', " &
                                 "0.000 AS 'Cost', " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'Points'," &
                                 "CASE WHEN 	DailySchedule.IDAssignment NOT IN " &
                                        "(@SELECT# DailyCoverage.IDAssignment " &
                                         "FROM DailyCoverage INNER JOIN Groups " &
                                                "ON Groups.ID = DailyCoverage.IDGroup " &
                                         "WHERE Date = @CoverageDate AND " &
                                               "(Groups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR Groups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%')) " &
                                 "THEN 10000 ELSE 0 END + " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'OrderField' , '' as ConceptValue " &
                          "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                    "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN ShiftAssignments ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                    "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                                    "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN Assignments ON Assignments.ID = DailySchedule.IDAssignment  " &
                          "WHERE (sysrovwAllEmployeeGroups.CurrentEmployee = 1) AND (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                                "sysrovwAllEmployeeGroups.IDEmployee <> @IDEmployee AND " &
                                "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                                "DailySchedule.Date = @CoverageDate AND ShiftAssignments.IDAssignment = @IDAssignment AND " &
                                "EmployeeAssignments.IDAssignment = @IDAssignment AND " &
                                "DailySchedule.IDAssignment <> @IDAssignment "
                strSQL &= "UNION "

                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                 "DailySchedule.IDAssignment AS 'IDAssignment', Assignments.Name AS 'AssignmentName', " &
                                 "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                 "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'Suitability', " &
                                 "0.000 AS 'Coverage', " &
                                 "0 AS 'Cost', " &
                                 "0 AS 'Points'," &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'OrderField', '' as ConceptValue " &
                          "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                    "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                                    "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN Assignments ON Assignments.ID = DailySchedule.IDAssignment  " &
                          "WHERE (sysrovwAllEmployeeGroups.CurrentEmployee = 1) AND (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                                "sysrovwAllEmployeeGroups.IDEmployee <> @IDEmployee AND " &
                                "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                                "DailySchedule.Date = @CoverageDate AND " &
                                "EmployeeAssignments.IDAssignment = @IDAssignment AND " &
                                "DailySchedule.IDAssignment <> @IDAssignment AND " &
                                "DailySchedule.IDShift1 NOT IN (@SELECT# ShiftAssignments.IDShift FROM ShiftAssignments WHERE ShiftAssignments.IDAssignment = @IDAssignment)"

                strSQL &= "ORDER BY 'OrderField' DESC"

                tbRet = New DataTable
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@IDEmployee", DbType.Int64)
                AddParameter(cmd, "@IDGroup", DbType.Int64)
                AddParameter(cmd, "@CoverageDate", DbType.Date)
                AddParameter(cmd, "@IDAssignment", DbType.Int64)
                cmd.Parameters("@IDEmployee").Value = _IDEmployee
                cmd.Parameters("@IDGroup").Value = intIDGroup
                cmd.Parameters("@CoverageDate").Value = _CoverageDate.Date
                cmd.Parameters("@IDAssignment").Value = intIDAssignment

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.Fill(tbRet)

                ' Obtenemos el campo a utilizar para el coste del empleado
                Dim oParameters As New roParameters("OPTIONS", True)

                Dim strCostField As String = roTypes.Any2String(oParameters.Parameter(Parameters.EmployeeFieldCost))
                strCostField = strCostField.Replace("USR_", "")

                Dim intIDConcept As Integer = 0
                strSQL = "@SELECT# isnull(ID, 0) FROM Concepts WHERE Description like '%#HRS%' "
                intIDConcept = roTypes.Any2Integer(ExecuteScalar(strSQL))

                ' Obtenemos el coste de cada empleado
                For Each dRow As DataRow In tbRet.Rows
                    If strCostField.Length > 0 Then
                        Dim EmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(dRow("IDEmployee")), strCostField, _CoverageDate.Date, New VTUserFields.UserFields.roUserFieldState(_State.IDPassport))

                        If Not IsDBNull(EmployeeUserField.FieldValue) And Not EmployeeUserField.FieldValue Is Nothing Then
                            dRow("Cost") = Any2Double(CStr(EmployeeUserField.FieldValue).Replace(".", roConversions.GetDecimalDigitFormat))
                            dRow.AcceptChanges()
                        End If
                    End If

                    If intIDConcept > 0 Then
                        'ConceptValue
                        Dim oEmployeesConcept As DataTable = Concept.roConcept.GetAnualAccruals(roTypes.Any2Integer(dRow("IDEmployee")), _CoverageDate.Date, _State, , intIDConcept)
                        If oEmployeesConcept IsNot Nothing Then
                            For Each oRow As DataRow In oEmployeesConcept.Rows
                                dRow("ConceptValue") = oRow("TotalFormat")
                                dRow.AcceptChanges()
                            Next
                        End If
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::GetCoverageEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::GetCoverageEmployees")
            Finally

            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Define una cobertura para un empleado y fecha en concreto.
        ''' </summary>
        ''' <param name="_IDEmployeeCoverage">Código del empleado que realizará la cobertura.</param>
        ''' <param name="_IDEmployeeCovered">Código del empleado a cubrir.</param>
        ''' <param name="_CoverageDate">Fecha de la cobertura</param>
        ''' <param name="_State"></param>
        ''' <param name="_IDShift">Opcional. Código del horario a utilizar para el empleado que realizará la cobertura.</param>
        ''' <param name="_Audit"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AddEmployeeCoverage(ByVal _IDEmployeeCoverage As Integer, ByVal _IDEmployeeCovered As Integer, ByVal _CoverageDate As Date, ByVal _State As Employee.roEmployeeState, ByRef _LockedDayActionEmployeeCovered As LockedDayAction, ByRef _LockedDayActionEmployeeCoverage As LockedDayAction, ByRef _IDEmployeeLocked As Integer, Optional ByVal _IDShift As Integer = -1, Optional ByVal _Audit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim bolHasPermission As Boolean = True
                ' Verificamos los permisos del pasaporte actual sobre la planificación
                bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Calendar.Scheduler", Permission.Write, _IDEmployeeCovered)
                If bolHasPermission Then bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Calendar.Scheduler", Permission.Write, _IDEmployeeCoverage)

                If bolHasPermission Then

                    'Verificamos que los empleados de la cobertura tengan contrato activo
                    If roBusinessSupport.EmployeeWithContract(_IDEmployeeCovered, _State, _CoverageDate) AndAlso roBusinessSupport.EmployeeWithContract(_IDEmployeeCoverage, _State, _CoverageDate) Then

                        If Not roBusinessSupport.InPeriodOfFreezing(_CoverageDate, _IDEmployeeCovered, _State) Then

                            bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                            Dim strSQL As String

                            ' Obtenemos los datos de la planificación del empleado al que se quiere cubrir
                            strSQL = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & _IDEmployeeCovered.ToString & " AND Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                            Dim tbDailyScheduleCovered As New DataTable
                            Dim cmdCovered As DbCommand = CreateCommand(strSQL)
                            'AddParameter(cmd, "@EmployeeName", DbType.String, 50)
                            'cmd.Parameters("@EmployeeName").Value = strName
                            Dim daCovered As DbDataAdapter = CreateDataAdapter(cmdCovered, True)
                            daCovered.Fill(tbDailyScheduleCovered)

                            ' Obtenemos los datos de la planificación del empleado que tiene que hacer la cobertura
                            strSQL = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & _IDEmployeeCoverage.ToString & " AND Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                            Dim tbDailyScheduleCoverage As New DataTable
                            Dim cmdCoverage As DbCommand = CreateCommand(strSQL)
                            Dim daCoverage As DbDataAdapter = CreateDataAdapter(cmdCoverage, True)
                            daCoverage.Fill(tbDailyScheduleCoverage)

                            Dim intIDAssignment As Integer = -1

                            ' Verificamos el emplado a cubrir
                            If tbDailyScheduleCovered.Rows.Count = 1 Then

                                bolRet = True
                                If Any2Boolean(tbDailyScheduleCovered.Rows(0).Item("LockedDay")) Then
                                    ' El día a asignar está bloqueado.
                                    Select Case _LockedDayActionEmployeeCovered
                                        Case LockedDayAction.None
                                            ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                            _State.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                            bolRet = False
                                        Case LockedDayAction.ReplaceFirst
                                            ' Se tiene que asignar igualmente
                                            bolRet = True
                                            ''_LockedDayAction1 = LockedDayAction.None
                                        Case LockedDayAction.ReplaceAll
                                            ' Se tiene que asignar igualmente
                                            bolRet = True
                                        Case LockedDayAction.NoReplaceFirst
                                            ' No se tiene que asignar
                                            bolRet = False
                                            ''_LockedDayAction1 = LockedDayAction.None
                                        Case LockedDayAction.NoReplaceAll
                                            ' No se tiene que asignar
                                            bolRet = False
                                    End Select
                                    If Not bolRet Then _IDEmployeeLocked = _IDEmployeeCovered
                                End If

                                If bolRet Then
                                    If Any2Integer(tbDailyScheduleCovered.Rows(0).Item("IDAssignment")) > 0 Then
                                        If Any2Boolean(tbDailyScheduleCovered.Rows(0).Item("IsCovered")) = False Then
                                            If Any2Integer(tbDailyScheduleCovered.Rows(0).Item("OldIDAssignment")) = 0 Then

                                                intIDAssignment = tbDailyScheduleCovered.Rows(0).Item("IDAssignment")
                                                bolRet = True
                                            Else ' El empleado a cubrir está haciendo una cobertura
                                                bolRet = False
                                                _State.Result = EmployeeResultEnum.EmployeeCoveredIsCovering
                                            End If
                                        Else ' El empleado a cubrir ya está cubierto
                                            bolRet = False
                                            _State.Result = EmployeeResultEnum.EmployeeCoveredAlreadyCovered
                                        End If
                                    Else ' El empleado a cubrir no tiene puesto asignado
                                        bolRet = False
                                        _State.Result = EmployeeResultEnum.EmployeeCoveredNoAssignment
                                    End If
                                End If
                            Else ' No existe planificación para el empleado a cubrir
                                bolRet = False
                                _State.Result = EmployeeResultEnum.DailyScheduleCoveredNotExist
                            End If

                            ' Verificamos el empleado que tiene que realizar la cobertura
                            If bolRet AndAlso tbDailyScheduleCoverage.Rows.Count = 1 Then

                                If Any2Boolean(tbDailyScheduleCoverage.Rows(0).Item("LockedDay")) Then
                                    ' El día a asignar está bloqueado.
                                    Select Case _LockedDayActionEmployeeCoverage
                                        Case LockedDayAction.None
                                            ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                            _State.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                            bolRet = False
                                        Case LockedDayAction.ReplaceFirst
                                            ' Se tiene que asignar igualmente
                                            bolRet = True
                                            ''_LockedDayAction2 = LockedDayAction.None
                                        Case LockedDayAction.ReplaceAll
                                            ' Se tiene que asignar igualmente
                                            bolRet = True
                                        Case LockedDayAction.NoReplaceFirst
                                            ' No se tiene que asignar
                                            bolRet = False
                                            ''_LockedDayAction2 = LockedDayAction.None
                                        Case LockedDayAction.NoReplaceAll
                                            ' No se tiene que asignar
                                            bolRet = False
                                    End Select
                                    If Not bolRet Then _IDEmployeeLocked = _IDEmployeeCoverage
                                End If

                                If bolRet Then
                                    If Any2Boolean(tbDailyScheduleCoverage.Rows(0).Item("IsCovered")) = False Then
                                        If Any2Integer(tbDailyScheduleCoverage.Rows(0).Item("OldIDAssignment")) = 0 Then

                                            bolRet = True
                                        Else ' El empleado que tiene que realizar la cobertura ya está haciendo una cobertura
                                            bolRet = False
                                            _State.Result = EmployeeResultEnum.EmployeeCoverageAlreadyCovering
                                        End If
                                    Else ' El empleado que tiene que realizar la cobertura está cubierto
                                        bolRet = False
                                        _State.Result = EmployeeResultEnum.EmployeeCoverageIsCovered
                                    End If
                                End If

                            End If

                            If bolRet Then
                                ' Verificamos que el empleado que tiene que realizar la cobertura pueda realizar el puesto
                                If Not roBusinessSupport.ExistEmployeeAssignment(_IDEmployeeCoverage, intIDAssignment, _State) Then
                                    bolRet = False
                                    _State.Result = EmployeeResultEnum.EmployeeCoverageEmployeeAssignmentIncompatibility
                                End If
                            End If

                            If bolRet Then
                                ' Verificamos que el horario asignado al empleado que tiene que realizar la cobertura sea compatible con el puesto
                                Dim oShiftState As New Shift.roShiftState
                                roBusinessState.CopyTo(_State, oShiftState)
                                If _IDShift <= 0 Then
                                    If tbDailyScheduleCoverage.Rows.Count = 1 Then
                                        _IDShift = Any2Integer(tbDailyScheduleCoverage.Rows(0).Item("IDShift1"))
                                    End If
                                End If
                                If Not Shift.roShiftAssignment.ExistShiftAssignment(_IDShift, intIDAssignment, oShiftState) Then
                                    bolRet = False
                                    _State.Result = EmployeeResultEnum.EmployeeCoverageShiftAssignmentIncompatibility
                                End If
                            End If

                            If bolRet Then

                                tbDailyScheduleCovered.Rows(0).Item("IsCovered") = True
                                tbDailyScheduleCovered.Rows(0).Item("Status") = 0
                                daCovered.Update(tbDailyScheduleCovered)

                                Dim oRow As DataRow
                                If tbDailyScheduleCoverage.Rows.Count = 1 Then
                                    oRow = tbDailyScheduleCoverage.Rows(0)
                                Else
                                    oRow = tbDailyScheduleCoverage.NewRow
                                    oRow("IDEmployee") = _IDEmployeeCoverage
                                    oRow("Date") = _CoverageDate
                                    oRow("LockedDay") = False
                                    tbDailyScheduleCoverage.Rows.Add(oRow)
                                End If

                                If Not IsDBNull(oRow("IDAssignment")) Then oRow("OldIDAssignment") = oRow("IDAssignment")
                                oRow("IDAssignment") = intIDAssignment
                                If Not IsDBNull(oRow("IDShift1")) Then oRow("OldIDShift") = oRow("IDShift1")
                                oRow("IDShift1") = _IDShift
                                oRow("IsCovered") = False
                                oRow("IDEmployeeCovered") = _IDEmployeeCovered
                                oRow("Status") = 0
                                oRow("GUID") = ""

                                daCoverage.Update(tbDailyScheduleCoverage)

                                ' Notificamos el cambio
                                roConnector.InitTask(TasksType.DAILYSCHEDULE)

                                ' Marcamos para recálculo las dotaciones planificadas para el empleado cubierto y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                Dim oSchedulerState As New Scheduler.roSchedulerState
                                roBusinessState.CopyTo(_State, oSchedulerState)
                                bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, _IDEmployeeCovered, , _CoverageDate, True)
                                roBusinessState.CopyTo(oSchedulerState, _State)
                                If bolRet Then
                                    ' Marcamos para recálculo las reales planificadas para el empleado cubierto y fecha, y no notificamos al servidor.
                                    bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, _IDEmployeeCovered, , _CoverageDate, False)
                                    roBusinessState.CopyTo(oSchedulerState, _State)
                                End If

                                If bolRet Then
                                    ' Marcamos para recálculo las dotaciones planificadas para el empleado que realiza la cobertura y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                    roBusinessState.CopyTo(_State, oSchedulerState)
                                    bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, _IDEmployeeCoverage, , _CoverageDate, True)
                                    roBusinessState.CopyTo(oSchedulerState, _State)
                                    If bolRet Then
                                        ' Marcamos para recálculo las reales planificadas para el empleado que realiza la cobertura y fecha, y no notificamos al servidor.
                                        bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, _IDEmployeeCoverage, , _CoverageDate, False)
                                        roBusinessState.CopyTo(oSchedulerState, _State)
                                    End If
                                End If

                            End If
                        Else
                            _State.Result = EmployeeResultEnum.InPeriodOfFreezing
                        End If
                    Else
                        _State.Result = EmployeeResultEnum.EmployeeNoActiveContract
                    End If
                Else
                    _State.Result = EmployeeResultEnum.AccessDenied
                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roEmployees::AddEmployeeCoverage")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roEmployees::AddEmployeeCoverage")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina una cobertura para un empleado y fecha.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado al que se le quiere eliminar la cobertura (puede ser tanto el empleado que realiza la cobertura cómo el empleado que se está cubriendo).</param>
        ''' <param name="_CoverageDate">Fecha de la cobertura</param>
        ''' <param name="_State"></param>
        ''' <param name="_Audit"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RemoveEmployeeCoverage(ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByRef _LockedDayActionEmployeeCovered As LockedDayAction, ByRef _LockedDayActionEmployeeCoverage As LockedDayAction, ByRef _EmployeeType As Integer, ByRef _IDEmployeeLocked As Integer, ByVal _State As Employee.roEmployeeState, Optional ByVal _Audit As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                Dim strSQL As String

                Dim _IDEmployeeCoverage As Integer
                Dim _IDEmployeeCovered As Integer

                strSQL = "@SELECT# CASE ISNULL(IsCovered,0) WHEN 0 " &
                                    "THEN DailySchedule.IDEmployee " &
                                    "ELSE (@SELECT# DS.IDEmployee FROM DailySchedule DS WHERE DS.Date = DailySchedule.Date AND DS.IDEmployeeCovered = DailySchedule.IDEmployee ) END  AS 'IDEmployeeCoverage', " &
                                "CASE ISNULL(IsCovered,0) WHEN 0 " &
                                    "THEN DailySchedule.IDEmployeeCovered " &
                                    "ELSE DailySchedule.IDEmployee END  AS 'IDEmployeeCovered' " &
                         "FROM DailySchedule " &
                         "WHERE IDEmployee = " & _IDEmployee.ToString & " AND " &
                               "Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    bolRet = True
                    _IDEmployeeCoverage = Any2Integer(tb.Rows(0).Item("IDEmployeeCoverage"))
                    _IDEmployeeCovered = Any2Integer(tb.Rows(0).Item("IDEmployeeCovered"))
                Else
                    bolRet = False
                    _State.Result = EmployeeResultEnum.EmployeePlanNotExist
                End If

                If bolRet Then

                    ' Informa si el empleado que se ha pasado como parámetro (_IDEmployee) es el empleado que realiza la cobertura o el que está siendo cubierto.
                    If _IDEmployee = _IDEmployeeCovered Then
                        _EmployeeType = 0
                    Else
                        _EmployeeType = 1
                    End If

                    Dim bolHasPermission As Boolean = True
                    ' Verificamos los permisos del pasaporte actual sobre la planificación
                    bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Calendar.Scheduler", Permission.Write, _IDEmployeeCovered)
                    If bolHasPermission Then bolHasPermission = WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, "Calendar.Scheduler", Permission.Write, _IDEmployeeCoverage)

                    If bolHasPermission Then

                        'Verificamos que los empleados de la cobertura tengan contrato activo
                        If roBusinessSupport.EmployeeWithContract(_IDEmployeeCovered, _State, _CoverageDate) AndAlso roBusinessSupport.EmployeeWithContract(_IDEmployeeCoverage, _State, _CoverageDate) Then

                            If Not roBusinessSupport.InPeriodOfFreezing(_CoverageDate, _IDEmployeeCovered, _State) Then
                                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                                ' Obtenemos los datos de la planificación del empleado al que se quiere cubrir
                                strSQL = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & _IDEmployeeCovered.ToString & " AND Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                                Dim tbDailyScheduleCovered As New DataTable
                                Dim cmdCovered As DbCommand = CreateCommand(strSQL)
                                'AddParameter(cmd, "@EmployeeName", DbType.String, 50)
                                'cmd.Parameters("@EmployeeName").Value = strName
                                Dim daCovered As DbDataAdapter = CreateDataAdapter(cmdCovered, True)
                                daCovered.Fill(tbDailyScheduleCovered)

                                ' Obtenemos los datos de la planificación del empleado que tiene que hacer la cobertura
                                strSQL = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & _IDEmployeeCoverage.ToString & " AND Date = " & Any2Time(_CoverageDate).SQLSmallDateTime
                                Dim tbDailyScheduleCoverage As New DataTable
                                Dim cmdCoverage As DbCommand = CreateCommand(strSQL)
                                Dim daCoverage As DbDataAdapter = CreateDataAdapter(cmdCoverage, True)
                                daCoverage.Fill(tbDailyScheduleCoverage)

                                Dim intIDAssignment As Integer = -1

                                ' Verificamos el emplado a cubrir
                                If tbDailyScheduleCovered.Rows.Count = 1 Then

                                    If Any2Boolean(tbDailyScheduleCovered.Rows(0).Item("LockedDay")) Then
                                        ' El día a asignar está bloqueado.
                                        Select Case _LockedDayActionEmployeeCovered
                                            Case LockedDayAction.None
                                                ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                _State.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                                bolRet = False
                                            Case LockedDayAction.ReplaceFirst
                                                ' Se tiene que asignar igualmente
                                                bolRet = True
                                                ''_LockedDayAction1 = LockedDayAction.None
                                            Case LockedDayAction.ReplaceAll
                                                ' Se tiene que asignar igualmente
                                                bolRet = True
                                            Case LockedDayAction.NoReplaceFirst
                                                ' No se tiene que asignar
                                                bolRet = False
                                                ''_LockedDayAction1 = LockedDayAction.None
                                            Case LockedDayAction.NoReplaceAll
                                                ' No se tiene que asignar
                                                bolRet = False
                                        End Select
                                        If Not bolRet Then _IDEmployeeLocked = _IDEmployeeCovered
                                    End If
                                Else ' No existe planificación para el empleado a cubrir
                                    bolRet = False
                                    _State.Result = EmployeeResultEnum.DailyScheduleCoveredNotExist
                                End If

                                If bolRet And tbDailyScheduleCoverage.Rows.Count = 1 Then

                                    If Any2Boolean(tbDailyScheduleCoverage.Rows(0).Item("LockedDay")) Then
                                        ' El día a asignar está bloqueado.
                                        Select Case _LockedDayActionEmployeeCoverage
                                            Case LockedDayAction.None
                                                ' Se tendrá que preguntar al usuario, devolvemos estado de error y marcamos para que no se asigne
                                                _State.Result = EmployeeResultEnum.DailyScheduleLockedDay
                                                bolRet = False
                                            Case LockedDayAction.ReplaceFirst
                                                ' Se tiene que asignar igualmente
                                                bolRet = True
                                                ''_LockedDayAction2 = LockedDayAction.None
                                            Case LockedDayAction.ReplaceAll
                                                ' Se tiene que asignar igualmente
                                                bolRet = True
                                            Case LockedDayAction.NoReplaceFirst
                                                ' No se tiene que asignar
                                                bolRet = False
                                                ''_LockedDayAction2 = LockedDayAction.None
                                            Case LockedDayAction.NoReplaceAll
                                                ' No se tiene que asignar
                                                bolRet = False
                                        End Select
                                        If Not bolRet Then _IDEmployeeLocked = _IDEmployeeCoverage
                                    End If

                                End If

                                If bolRet Then

                                    tbDailyScheduleCovered.Rows(0).Item("IsCovered") = DBNull.Value
                                    tbDailyScheduleCovered.Rows(0).Item("Status") = 0
                                    daCovered.Update(tbDailyScheduleCovered)

                                    Dim oRow As DataRow
                                    If tbDailyScheduleCoverage.Rows.Count = 1 Then
                                        oRow = tbDailyScheduleCoverage.Rows(0)
                                    Else
                                        oRow = tbDailyScheduleCoverage.NewRow
                                        oRow("IDEmployee") = _IDEmployeeCoverage
                                        oRow("Date") = _CoverageDate
                                        oRow("LockedDay") = False
                                        tbDailyScheduleCoverage.Rows.Add(oRow)
                                    End If

                                    oRow("IDAssignment") = Any2Integer(oRow("OldIDAssignment"))
                                    oRow("OldIDAssignment") = DBNull.Value
                                    oRow("IDShift1") = oRow("OldIDShift")
                                    oRow("OldIDShift") = DBNull.Value
                                    oRow("IsCovered") = DBNull.Value
                                    oRow("IDEmployeeCovered") = DBNull.Value
                                    oRow("Status") = 0
                                    oRow("GUID") = ""

                                    daCoverage.Update(tbDailyScheduleCoverage)

                                    ' Notificamos el cambio
                                    roConnector.InitTask(TasksType.DAILYSCHEDULE)

                                    ' Marcamos para recálculo las dotaciones planificadas para el empleado cubierto y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                    Dim oSchedulerState As New Scheduler.roSchedulerState
                                    roBusinessState.CopyTo(_State, oSchedulerState)
                                    bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, _IDEmployeeCovered, , _CoverageDate, True)
                                    roBusinessState.CopyTo(oSchedulerState, _State)
                                    If bolRet Then
                                        ' Marcamos para recálculo las reales planificadas para el empleado cubierto y fecha, y no notificamos al servidor.
                                        bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, _IDEmployeeCovered, , _CoverageDate, False)
                                        roBusinessState.CopyTo(oSchedulerState, _State)
                                    End If

                                    If bolRet Then
                                        ' Marcamos para recálculo las dotaciones planificadas para el empleado que realiza la cobertura y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                                        roBusinessState.CopyTo(_State, oSchedulerState)
                                        bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, _IDEmployeeCoverage, , _CoverageDate, True)
                                        roBusinessState.CopyTo(oSchedulerState, _State)
                                        If bolRet Then
                                            ' Marcamos para recálculo las reales planificadas para el empleado que realiza la cobertura y fecha, y no notificamos al servidor.
                                            bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, _IDEmployeeCoverage, , _CoverageDate, False)
                                            roBusinessState.CopyTo(oSchedulerState, _State)
                                        End If
                                    End If

                                End If
                            Else
                                _State.Result = EmployeeResultEnum.InPeriodOfFreezing
                            End If
                        Else
                            _State.Result = EmployeeResultEnum.EmployeeNoActiveContract
                        End If
                    Else
                        _State.Result = EmployeeResultEnum.AccessDenied
                    End If

                End If
            Catch ex As DbException
                bolRet = False
                _State.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeCoverage")
            Catch ex As Exception
                bolRet = False
                _State.UpdateStateInfo(ex, "roEmployees::RemoveEmployeeCoverage")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de posibles empleados que pueden realizar un puesto para poder planificar una dotación para un grupo, puesto y fecha concretos.
        ''' </summary>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_IDAssignment"></param>
        ''' <param name="_CoverageDate"></param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDailyCoverageEmployees(ByVal _IDGroup As Integer, ByVal _IDAssignment As Integer, ByVal _CoverageDate As Date, ByVal _State As Employee.roEmployeeState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String

                ' Empleados sin horario asignado y que puedan cubrir el puesto
                strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                         "NULL AS 'IDAssignment', '' AS 'AssignmentName', " &
                         "NULL AS 'IDShift1', '' AS 'ShiftName', " &
                         "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                         "(@SELECT# EmployeeAssignments.Suitability FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment = @IDAssignment) AS  'Suitability', NULL AS 'Coverage', 0.000 AS 'Cost', NULL AS 'Points'," &
                         "40000 AS 'OrderField', '' as ConceptValue " &
                         "FROM sysrovwAllEmployeeGroups INNER JOIN EmployeeContracts ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                         "WHERE (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                         "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                         "ISNULL((@SELECT# COUNT(*) FROM DailySchedule WHERE DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND DailySchedule.Date = @CoverageDate AND ISNULL(DailySchedule.IDShift1, 0) > 0), 0) = 0 AND " &
                         "(@SELECT# COUNT(*) FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment = @IDAssignment) = 1 AND " &
                         "(EmployeeContracts.EndDate >= @CoverageDate) AND (EmployeeContracts.BeginDate <= @CoverageDate) "

                strSQL &= " UNION "

                ' Empleados con horario asignado compatible con el puesto, sin puesto asignado y que puedan cubrir el puesto
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                 "NULL AS 'IDAssignment', '' AS 'AssignmentName', " &
                                 "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                 "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'Suitability', " &
                                 "ISNULL(ShiftAssignments.Coverage,0) * 100 AS 'Coverage', " &
                                 "0.000 AS 'Cost', " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'Points', " &
                                 "20000 + (ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100)) AS 'OrderField' , '' as ConceptValue " &
                          "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                    "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN ShiftAssignments ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                    "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                                    "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN EmployeeContracts ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                          "WHERE (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                                "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                                "DailySchedule.Date = @CoverageDate AND ShiftAssignments.IDAssignment = @IDAssignment AND ISNULL(DailySchedule.IDAssignment,0) = 0 AND EmployeeAssignments.IDAssignment = @IDAssignment AND " &
                                "(EmployeeContracts.EndDate >= @CoverageDate) AND (EmployeeContracts.BeginDate <= @CoverageDate) "

                strSQL &= " UNION "

                ' Empleados con horario asignado compatible con el puesto, con puesto asignado distinto al puesto a cubrir (primero los que el puesto asignadono intervenga en ninguna de las dotaciones) y que puedan cubrir el puesto.
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                 "DailySchedule.IDAssignment AS 'IDAssignment', Assignments.Name AS 'AssignmentName', " &
                                 "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                 "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) AS 'Suitability', " &
                                 "ISNULL(ShiftAssignments.Coverage,0) * 100 AS 'Coverage', " &
                                 "0.000 AS 'Cost', " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'Points'," &
                                 "CASE WHEN 	DailySchedule.IDAssignment NOT IN " &
                                        "(@SELECT# DailyCoverage.IDAssignment " &
                                         "FROM DailyCoverage INNER JOIN Groups " &
                                                "ON Groups.ID = DailyCoverage.IDGroup " &
                                         "WHERE Date = @CoverageDate AND " &
                                               "(Groups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR Groups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%')) " &
                                 "THEN 10000 ELSE 0 END + " &
                                 "ISNULL(EmployeeAssignments.Suitability,0) * (ISNULL(ShiftAssignments.Coverage,0) * 100) AS 'OrderField' , '' as ConceptValue " &
                          "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                    "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN ShiftAssignments ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                    "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                                    "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                    "INNER JOIN Assignments ON Assignments.ID = DailySchedule.IDAssignment  " &
                                    "INNER JOIN EmployeeContracts ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                          "WHERE (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = @IDGroup) + '\%') AND " &
                                "sysrovwAllEmployeeGroups.BeginDate <= @CoverageDate AND sysrovwAllEmployeeGroups.EndDate >= @CoverageDate AND " &
                                "DailySchedule.Date = @CoverageDate AND ShiftAssignments.IDAssignment = @IDAssignment AND " &
                                "EmployeeAssignments.IDAssignment = @IDAssignment AND DailySchedule.IDAssignment <> @IDAssignment AND " &
                                "(EmployeeContracts.EndDate >= @CoverageDate) AND (EmployeeContracts.BeginDate <= @CoverageDate)"

                strSQL &= "ORDER BY 'OrderField' DESC"

                tbRet = New DataTable
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@IDGroup", DbType.Int64)
                AddParameter(cmd, "@CoverageDate", DbType.Date)
                AddParameter(cmd, "@IDAssignment", DbType.Int64)
                cmd.Parameters("@IDGroup").Value = _IDGroup
                cmd.Parameters("@CoverageDate").Value = _CoverageDate.Date
                cmd.Parameters("@IDAssignment").Value = _IDAssignment

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.Fill(tbRet)

                ' Obtenemos el campo a utilizar para el coste del empleado
                Dim oParameters As New roParameters("OPTIONS", True)

                Dim strCostField As String = roTypes.Any2String(oParameters.Parameter(Parameters.EmployeeFieldCost))
                strCostField = strCostField.Replace("USR_", "")

                Dim intIDConcept As Integer = 0
                strSQL = "@SELECT# isnull(ID, 0) FROM Concepts WHERE Description like '%#HRS%' "
                intIDConcept = roTypes.Any2Integer(ExecuteScalar(strSQL))

                ' Obtenemos el coste de cada empleado
                For Each dRow As DataRow In tbRet.Rows

                    If strCostField.Length > 0 Then
                        Dim EmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(dRow("IDEmployee")), strCostField, _CoverageDate.Date, New VTUserFields.UserFields.roUserFieldState(_State.IDPassport))

                        If Not IsDBNull(EmployeeUserField.FieldValue) And Not EmployeeUserField.FieldValue Is Nothing Then
                            dRow("Cost") = Any2Double(CStr(EmployeeUserField.FieldValue).Replace(".", roConversions.GetDecimalDigitFormat))
                            dRow.AcceptChanges()
                        End If
                    End If

                    If intIDConcept > 0 Then
                        'ConceptValue
                        Dim oEmployeesConcepttb As DataTable = Concept.roConcept.GetAnualAccruals(roTypes.Any2Integer(dRow("IDEmployee")), _CoverageDate.Date, _State, , intIDConcept)
                        If oEmployeesConcepttb IsNot Nothing Then
                            For Each oRow As DataRow In oEmployeesConcepttb.Rows
                                dRow("ConceptValue") = oRow("TotalFormat")
                                dRow.AcceptChanges()
                            Next
                        End If
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::GetDailyCoverageEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::GetDailyCoverageEmployees")
            Finally

            End Try

            Return tbRet

        End Function

        ''' <summary>
        ''' Devuelve un array de bytes del fichero especificado como paramentro.
        ''' </summary>
        ''' <param name="strFileName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDocumentToView(ByVal strFileName As String, ByRef oState As Employee.roEmployeeState) As Byte()

            Dim strFileNameTemp As String = strFileName
            Dim arrBytes As Byte() = Nothing
            Dim bolRet As Boolean = False

            Dim bExitsFile As Boolean = False

            Try

                If strFileNameTemp <> String.Empty Then
                    If roBusinessSupport.IsValidFileNameOrPath(strFileNameTemp) Then

                        'obtener carpeta de documentos de la clave de registro PATH/DOCUMENTS
                        Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                        Dim strPathTemplates As String = oSettings.GetVTSetting(eKeys.Documents)

                        'comprobar si existe el fichero en el directorio DOCUMENTS
                        Dim myDir As New IO.DirectoryInfo(strPathTemplates)
                        If myDir.Exists Then
                            strFileNameTemp = System.IO.Path.Combine(strPathTemplates, strFileNameTemp)
                            If System.IO.File.Exists(strFileNameTemp) Then
                                bExitsFile = True
                            End If
                        End If

                        If bExitsFile = False Then
                            'comprobar si exite el fichero en el path absoluto
                            If System.IO.File.Exists(strFileName) Then
                                bExitsFile = True
                            End If
                        End If

                        If bExitsFile = True Then

                            Dim Permission As New System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Read, strFileNameTemp)
                            Try
                                Permission.Demand()

                                Dim objFileStream As System.IO.FileStream = Nothing

                                Try
                                    objFileStream = New System.IO.FileStream(strFileNameTemp, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                                    ReDim arrBytes(objFileStream.Length - 1)
                                    objFileStream.Read(arrBytes, 0, arrBytes.Length)

                                    bolRet = True
                                Catch Ex As Exception
                                    oState.UpdateStateInfo(Ex, "roEmployees::GetDocumentToView")

                                End Try
                            Catch s As System.Security.SecurityException
                                oState.Result = EmployeeResultEnum.FileWithoutPermissionToRead
                            End Try
                        Else
                            oState.Result = EmployeeResultEnum.FileNotExists
                        End If
                    Else
                        oState.Result = EmployeeResultEnum.FileInvalidName
                    End If
                Else
                    oState.Result = EmployeeResultEnum.FileNotEspecified
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetDocumentToView")
            End Try

            If bolRet Then
                Return arrBytes
            Else
                Return Nothing
            End If

        End Function

        Public Shared Function GetMobilitiesList(ByVal IDEmployee As Integer, ByVal BeginDate As DateTime, ByVal EndDate As DateTime, ByRef oState As Employee.roEmployeeState) As Generic.List(Of Employee.roEmployeeMobility)

            Dim lstEmployeeMobilities As New Generic.List(Of Employee.roEmployeeMobility)

            Try

                Dim strSQL As String = "@SELECT# IDGroup, BeginDate, EndDate FROM EmployeeGroups WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "((EmployeeGroups.BeginDate <= " & Any2Time(BeginDate).SQLSmallDateTime & " AND EmployeeGroups.EndDate >= " & Any2Time(BeginDate).SQLSmallDateTime & ") OR " &
                                       " (EmployeeGroups.BeginDate <= " & Any2Time(EndDate).SQLSmallDateTime & " AND EmployeeGroups.EndDate >= " & Any2Time(EndDate).SQLSmallDateTime & ") OR " &
                                       " (EmployeeGroups.BeginDate > " & Any2Time(BeginDate).SQLSmallDateTime & " AND EmployeeGroups.EndDate < " & Any2Time(EndDate).SQLSmallDateTime & ")) " &
                                       "ORDER BY BeginDate"
                Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
                While rd.Read()
                    lstEmployeeMobilities.Add(New Employee.roEmployeeMobility(rd("IDGroup"), rd("BeginDate"), rd("EndDate")))
                End While

                rd.Close()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMobilitiesList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMobilitiesList")
            Finally

            End Try

            Return lstEmployeeMobilities

        End Function

        Public Shared Function EmployeeOHPDocumentationExpired(ByVal IDEmployee As Integer, ByVal dDate As DateTime, ByRef oState As Employee.roEmployeeState) As Boolean

            Dim bRet As Boolean = False

            Try

                Dim tbRet As New DataTable
                Dim Command As DbCommand = CreateCommand("EmployeeDocuments")
                Command.CommandType = CommandType.StoredProcedure
                AddParameter(Command, "@idEmployee", DbType.Int32).Value = IDEmployee
                AddParameter(Command, "@date", DbType.DateTime).Value = dDate.Date
                Dim Adapter As DbDataAdapter = CreateDataAdapter(Command)
                Adapter.Fill(tbRet)
                For Each row As DataRow In tbRet.Rows
                    If Any2Boolean(row.Item("Expired")) Then
                        Return True
                    End If
                Next
            Catch ex As DbException
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roEmployees::EmployeeOHPDocumentsStatus")
            Catch ex As Exception
                If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roEmployees::EmployeeOHPDocumentsStatus")
            Finally

            End Try

            Return bRet

        End Function

#End Region

    End Class

End Namespace