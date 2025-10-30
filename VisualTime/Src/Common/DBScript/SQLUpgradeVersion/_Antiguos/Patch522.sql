--Informe calendario semanal para usuarios (Verdecora)
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia1] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia2] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia3] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia4] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia5] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia6] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [horariodia7] [NVARCHAR](128) NOT NULL DEFAULT ''
GO
ALTER TABLE [dbo].[TMPCALENDAREMPLOYEEWeekly] ADD [section] [NVARCHAR](128) NOT NULL DEFAULT ''
GO



ALTER PROCEDURE [dbo].[Genius_EfectiveWork] @initialDate smalldatetime, @endDate smalldatetime, @idpassport int,  @userFieldsFilter nvarchar(max), @employeeFilter nvarchar(max) AS  

		
  		DECLARE @employeeIDs Table(idEmployee int)
		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
		DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter
		DECLARE @intdatefirst int  
  		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  		SET DateFirst @intdatefirst;  
  		
		INSERT INTO @employeeIDs EXEC dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
		
		SELECT	IDEmployee,
				EmployeeName,
				GroupName, 
				FullGroupName,
				WorkCenter,
				CASE WHEN InAbsence = 1 OR NoWork = 1 THEN '' ELSE CASE WHEN TelecommutingExpected = 1 THEN 'X' ELSE '' END END AS TelecommutingExpected,
				InAbsence,
				NoWork,
				IDContract,
				BeginContract_ToDateString,
				EndContract_ToDateString,
				Age, 
				RegDate AS Date_ToDateString,
				Mes,
				Año,
				DayOfWeek,
				WeekOfYear,
				DayOfYear,
				Quarter,
				Nivel1, 
				Nivel2, 
				Nivel3, 
				Nivel4, 
				Nivel5, 
				Nivel6,
				Nivel7, 
				Nivel8, 
				Nivel9, 
				Nivel10, 
				UserField1, 
				UserField2, 
				UserField3, 
				UserField4, 
				UserField5, 
				UserField6, 
				UserField7, 
				UserField8, 
				UserField9, 
				UserField10, 
				Office as Office_ToHours, 
				ISNULL(Telecommute,0) AS Telecommute_ToHours 
		FROM
		(
			SELECT Employees.Id AS IdEmployee,
				dt AS RegDate, 
				MONTH(dt) AS Mes,   
  				YEAR(dt) AS Año, 
				(DATEPART(dw, dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
				DATEPART(iso_week, dt) AS WeekOfYear, 
				DATEPART(dy, dt) AS DayOfYear, 
				DATEPART(QUARTER, dt) AS Quarter, 
				Employees.Name as EmployeeName, 
				dbo.sysroEmployeeGroups.GroupName, 
				dbo.sysroEmployeeGroups.FullGroupName, 
				EmployeeContracts.Enterprise As WorkCenter,
				ISNULL(DailySchedule.Telecommuting,CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, dt) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END) AS TelecommutingExpected,
				EmployeeContracts.IDContract AS IDContract,
				EmployeeContracts.BeginDate AS BeginContract_ToDateString,
				EmployeeContracts.EndDate AS EndContract_ToDateString,
				ISNULL(sysrovwDailyEfectiveWorkingHours.Value,0.0) As Value, 
				CASE WHEN sysrovwDailyEfectiveWorkingHours.InTelecommute = 1 THEN 'Telecommute' ELSE 'Office' END AS TimeType,
				CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END AS InAbsence,
			    CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END AS NoWork,
			  	dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,
				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dt) As UserField1,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dt) As UserField2,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dt) As UserField3,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dt) As UserField4,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dt) As UserField5,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dt) As UserField6,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dt) As UserField7,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dt) As UserField8,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dt) As UserField9,  
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.IdEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dt) As UserField10,
 				dbo.GetEmployeeAge(Employees.Id) As Age    
			FROM [dbo].[Alldays] (@pinitialDate, @pendDate)
			LEFT JOIN EmployeeContracts with (nolock) ON dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
			LEFT JOIN sysroEmployeeGroups with (nolock) ON dt BETWEEN sysroEmployeeGroups.BeginDate AND sysroEmployeeGroups.EndDate AND EmployeeContracts.IDEmployee = sysroEmployeeGroups.IDEmployee
			LEFT JOIN Employees with (nolock) ON Employees.ID = sysroEmployeeGroups.IDEmployee
			LEFT JOIN DailySchedule with (nolock) ON DailySchedule.IDEmployee =  Employees.ID AND DailySchedule.Date = dt
			LEFT JOIN sysrovwDailyEfectiveWorkingHours with (nolock) ON sysrovwDailyEfectiveWorkingHours.Date = dt AND sysrovwDailyEfectiveWorkingHours.IdEmployee = Employees.ID
			LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND dt BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
			LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = DailySchedule.IDShift1
			WHERE sysroEmployeeGroups.IdEmployee IN (SELECT idemployee FROM @employeeIDs)
			AND dbo.WebLogin_GetPermissionOverEmployee(@idpassport,Employees.Id,2,0,0,dt) > 1 
		) TMP
		PIVOT (AVG(Value) for TimeType in (Office,Telecommute)) AS Accrual
		ORDER BY IdEmployee ASC, RegDate ASC
GO



 ALTER VIEW [dbo].[sysroEmployeesShifts]  
    AS  
    SELECT        dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path,   
                             dbo.Shifts.ID AS IDShift, dbo.Shifts.Name AS ShiftName, (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN 0 ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours,   
                             dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, dbo.sysroEmployeeGroups.SecurityFlags,   
                             dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating, 
                             CASE WHEN dbo.DailySchedule.Date <= GETDATE() THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay, dbo.DailySchedule.IDAssignment, 
                             CASE when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,  
                             (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,  
                             (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,  
                             dbo.sysroRemarks.Text as Remark,  
                             (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN isnull(dbo.DailySchedule.ExpectedWorkingHours, (select isnull(ExpectedWorkingHours,0) from Shifts where id = dbo.DailySchedule.IDShiftBase)) ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHoursAbs,
							 DailySchedule.Telecommuting
    FROM         dbo.DailySchedule with (nolock)  INNER JOIN
                             dbo.Employees with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID INNER JOIN  
                             dbo.EmployeeContracts with (nolock) ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN  
                             dbo.sysroEmployeeGroups with (nolock) ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate AND dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN  
                             dbo.Shifts with (nolock) ON dbo.Shifts.ID =  (SELECT  CASE WHEN Date <= GETDATE() THEN IDShiftUsed ELSE IDShift1 END AS Expr1 FROM dbo.DailySchedule AS DS with (nolock) WHERE (Date = dbo.DailySchedule.Date) AND (IDEmployee = dbo.DailySchedule.IDEmployee))
                 LEFT OUTER JOIN dbo.sysroRemarks with (nolock) ON dbo.sysroRemarks.ID = dbo.DailySchedule.remarks
GO


 
 
 ALTER PROCEDURE  [dbo].[Genius_Schedule]
         	@initialDate smalldatetime,
         	@endDate smalldatetime,
         	@idpassport int,
         	@employeeFilter nvarchar(max),
         	@userFieldsFilter nvarchar(max)
          AS
          DECLARE @employeeIDs Table(idEmployee int)
      	 DECLARE @pinitialDate smalldatetime = @initialDate,  
           @pendDate smalldatetime = @endDate,  
           @pidpassport int = @idpassport,  
           @pemployeeFilter nvarchar(max) = @employeeFilter,  
           @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
   		
   		DECLARE @intdatefirst int
   		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
   		SET DateFirst @intdatefirst;
           insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
           SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
      		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
      		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName,
			CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting,CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, CurrentDate) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END)) = 1 THEN 'X' ELSE '' END END AS TelecommutingExpected,
      		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
      		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
      		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
      		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, DATEPART(QUARTER, dbo.sysroEmployeesShifts.CurrentDate) as Quarter, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
      		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
      		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
      		dbo.sysroEmployeesShifts.IDAssignment,
      		convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark) as Remark,
      		dbo.sysroEmployeesShifts.AssignmentName,
      		dbo.sysroEmployeesShifts.IDProductiveUnit,
      		dbo.sysroEmployeesShifts.ProductiveUnitName, 
    		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
      		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField1,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField2,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField3,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField4,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField5,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField6,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField7,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField8,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField9,
      		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField10,
 			dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate
          FROM dbo.sysroEmployeesShifts with (nolock) 
      		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
      		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
			LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = sysroEmployeesShifts.IDShift
      		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
     		LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND CurrentDate BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
      	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
      		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
      	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
      		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
      		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, 
      		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
      		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
      		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
      		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark),
			CASE WHEN (CASE WHEN ProgrammedAbsences.IDEmployee IS NULL THEN 0 ELSE 1 END) = 1 OR CASE WHEN Shifts.ID IS NULL OR Shifts.ShiftType = 2 OR Shifts.ExpectedWorkingHours = 0 THEN 1 ELSE 0 END = 1 THEN '' ELSE CASE WHEN (ISNULL(sysroEmployeesShifts.Telecommuting,CASE WHEN (Employees.Telecommuting = 1 AND ISNULL(Employees.TelecommutingDays,'') = '') THEN 1 ELSE (CASE WHEN CHARINDEX(CONVERT(VARCHAR,DATEPART(weekday, CurrentDate) - 1), TelecommutingDays) > 0 THEN 1 ELSE 0 END) END)) = 1 THEN 'X' ELSE '' END END
GO

UPDATE GeniusViews SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"TelecommutingExpected"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Hours\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-10-27T07:27:37.854Z"}' WHERE Name = 'scheduleByEmployeeDate'
GO

UPDATE sysroParameters SET Data='522' WHERE ID='DBVersion'
GO


 