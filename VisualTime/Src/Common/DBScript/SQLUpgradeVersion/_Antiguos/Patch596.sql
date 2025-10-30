ALTER TABLE DailySchedule ADD TelecommutingOptional BIT DEFAULT NULL
GO

ALTER FUNCTION [dbo].[EmployeeZonesBetweenDates]
   (				
 	@datebeginpar smalldatetime,
 	@dateendpar smalldatetime,
	@employeeidlist nvarchar(max)
   )
   RETURNS @ValueTable table(IDEmployee int, IDContract NVARCHAR(50), TelecommutingExpected BIT, TelecommutePlanned BIT, TelecommuteOptional BIT, InTelecommute BIT, RefDate smalldatetime, ContractWorkCenter NVARCHAR(50),
							 DailyWorkCenter NVARCHAR(50), CalculatedWorkCenter NVARCHAR(50), InAbsence BIT, NoWork BIT, ZoneOnDate NVARCHAR(50), ExpectedZone NVARCHAR(50)) 
   AS
   BEGIN
    declare @iniperiod smalldatetime 
 	declare @endperiod smalldatetime
	DECLARE @employees nvarchar(max)
	DECLARE @employeeIDs Table(idEmployee int)
 	SET @iniperiod = @datebeginpar
 	SET @endperiod = @dateendpar
	SET @employees = @employeeidlist

	IF LEN(@employees) = 0 
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 0 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   DailySchedule.TelecommutingOptional AS TelecommuteOptional,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
	ELSE
		INSERT INTO @ValueTable
		SELECT Employees.Id AS IDEmployee,
			   EmployeeContracts.IDContract,
			   CASE WHEN (Agreement.Telecommuting = 1 AND ISNULL(Agreement.TelecommutingMandatoryDays,'') = '') THEN 0 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,(DATEPART(dw, dt) + @@DATEFIRST - 1 ) % 7), Agreement.TelecommutingMandatoryDays) > 0 THEN 1 ELSE 0 END) END AS TelecommutingExpected,
			   DailySchedule.Telecommuting AS TelecommutePlanned,
			   DailySchedule.TelecommutingOptional AS TelecommuteOptional,
			   ISNULL(PunchesTC.ID,0) InTelecommute,
			   dt AS [RefDate],
			   ISNULL(EmployeeContracts.Enterprise,'') as ContractWorkCenter,
			   ISNULL(DailySchedule.WorkCenter,'') as DailyWorkCenter,
			   CASE WHEN DailySchedule.WorkCenter IS NOT NULL THEN ISNULL(DailySchedule.WorkCenter,'') ELSE ISNULL(EmployeeContracts.Enterprise,'') END AS CalculatedWorkCenter,
			   CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			   CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			   CASE WHEN Punches_LastOnDate.ActualType IS NOT NULL AND Punches_LastOnDate.ActualType <> 2 THEN  ISNULL(ZoneOnPastDate.Name,'?')  END AS ZoneOnDate,
			   CASE WHEN Punches_LastOnPastDates.ActualType IS NOT NULL AND Punches_LastOnPastDates.ActualType <> 2 AND dt >= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) THEN ISNULL(ZoneExpectedOnDate.Name,'?') END  AS ExpectedZone
		FROM   [dbo].[Alldays] (@iniperiod, @endperiod)
			   LEFT JOIN EmployeeContracts WITH(NOLOCK) ON alldays.dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			   LEFT JOIN sysrovwTelecommutingAgreement Agreement ON Agreement.IDContract = EmployeeContracts.IDContract 
			   INNER JOIN Employees WITH(NOLOCK) ON Employees.ID = EmployeeContracts.IDEmployee
			   LEFT JOIN DailySchedule WITH(NOLOCK) ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = EmployeeContracts.IDemployee
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberTC', * FROM Punches WITH(NOLOCK) WHERE InTelecommute = 1 AND Shiftdate >= @iniperiod) PunchesTC ON PunchesTC.IDEmployee = EmployeeContracts.IDemployee AND PunchesTC.ShiftDate = dt AND (RowNumberTC = 1 OR RowNumberTC IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Shiftdate ORDER BY DateTime DESC) AS 'RowNumberLastOnDate', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= @iniperiod) Punches_LastOnDate ON Punches_LastOnDate.IDEmployee = EmployeeContracts.IDemployee AND Punches_LastOnDate.ShiftDate = dt AND (RowNumberLastOnDate = 1 OR RowNumberLastOnDate IS NULL)
			   LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY DateTime DESC) AS 'RowNumberLastOnPastDates', * FROM Punches WITH(NOLOCK) WHERE IDZone > 0 AND ActualType <> 2 AND Shiftdate >= DATEADD(day,-30,@iniperiod) AND ShiftDate <= DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0)) Punches_LastOnPastDates ON Punches_LastOnPastDates.IDEmployee = EmployeeContracts.IDemployee AND (RowNumberLastOnPastDates = 1 OR RowNumberLastOnPastDates IS NULL)
			   LEFT JOIN Zones AS ZoneOnPastDate ON ZoneOnPastDate.ID = Punches_LastOnDate.IDZone
			   LEFT JOIN Zones AS ZoneExpectedOnDate ON ZoneExpectedOnDate.ID = Punches_LastOnPastDates.IDZone
			   LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			   LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
        WHERE EmployeeContracts.IDEmployee IN (SELECT Value AS IDEmployye FROM SplitToInt(@employees,','))
	RETURN
   END
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='596' WHERE ID='DBVersion'
GO

