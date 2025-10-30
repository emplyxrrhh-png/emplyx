IF NOT EXISTS (SELECT * FROM sysroLiveAdvancedParameters WHERE ParameterName = 'Customization' AND Value = 'TAIF')  
BEGIN
	 EXEC sp_executesql N'ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverGroup] 
    	(
    		@idPassport int,
    		@idGroup int,
    		@idApplication int,
    		@mode int --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
    	)
    RETURNS int
    AS
    BEGIN
    	DECLARE @Result int
  	DECLARE @parentPassport int
    	DECLARE @GroupType nvarchar(50)
   	
  	SELECT @GroupType = isnull(GroupType, '''') FROM sysroPassports WHERE ID = @idPassport
    		
    	if @GroupType = ''U''
    	begin
    		SET @parentPassport = @idPassport
    	end
    	else
    	begin
    		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
    	end
  	SELECT @Result = Permission FROM sysroPermissionsOverGroups WHERE PassportID = @parentPassport AND EmployeeGroupID = @idGroup AND EmployeeFeatureID = @idApplication

  	RETURN @Result
    END'
	EXEC sp_executesql N' ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee]    
  	(@idPassport int, @idEmployee int, @idApplication int, @mode int, @includeGroups bit, @date datetime)  RETURNS int    
        AS    
  	BEGIN    
  		DECLARE @Result int    
  		DECLARE @parentPassport int    
  		DECLARE @GroupType nvarchar(50),  
  		@pidPassport int = @idPassport,    
          @pidEmployee int = @idEmployee,    
          @pidApplication int = @idApplication,    
          @pmode int = @mode,  
          @pincludeGroups bit = @includeGroups ,  
          @pdate datetime   = @date,
  		@employeeResult int,
  		@calendarlock int
  	SELECT @GroupType = isnull(GroupType, '''') FROM sysroPassports WHERE ID = @pidPassport   
  	 
      IF @GroupType = ''U''    
      BEGIN    
          SET @parentPassport = @pidPassport    
      END    
      ELSE    
      BEGIN    
          SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @pidPassport    
      END    
  	SELECT @Result = Permission, 
  		@employeeResult = isnull((select poe.Permission from sysroPermissionsOverEmployeesExceptions poe where poe.PassportID = @parentPassport and poe.EmployeeFeatureID = @pidApplication and poe.EmployeeID = @pidEmployee),9)
  		FROM sysroPermissionsOverGroups where EmployeeGroupID = dbo.GetEmployeeGroup(@pidEmployee,@pdate) 
  			and sysroPermissionsOverGroups.EmployeeFeatureID =  @pidApplication and sysroPermissionsOverGroups.PassportID = @parentPassport
  	IF @Result > @employeeResult
  	BEGIN
  	 SET @Result = @employeeResult
  	END
  	
  	RETURN @Result    
  END'
END 
go


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
                            (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN isnull(dbo.DailySchedule.ExpectedWorkingHours, (select isnull(ExpectedWorkingHours,0) from Shifts where id = dbo.DailySchedule.IDShiftBase)) ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHoursAbs  
   FROM         dbo.DailySchedule with (nolock)  INNER JOIN
                            dbo.Employees with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID INNER JOIN  
                            dbo.EmployeeContracts with (nolock) ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN  
                            dbo.sysroEmployeeGroups with (nolock) ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate AND dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN  
                            dbo.Shifts with (nolock) ON dbo.Shifts.ID =  (SELECT  CASE WHEN Date <= GETDATE() THEN IDShiftUsed ELSE IDShift1 END AS Expr1 FROM dbo.DailySchedule AS DS with (nolock) WHERE (Date = dbo.DailySchedule.Date) AND (IDEmployee = dbo.DailySchedule.IDEmployee))
                LEFT OUTER JOIN dbo.sysroRemarks with (nolock) ON dbo.sysroRemarks.ID = dbo.DailySchedule.remarks
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 72)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType,IDCategory) VALUES(72,'Employee forgot exit punch',null, 5, 1,'','',6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 1803)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (1803,72,'Employee forgotten exit','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,1,0,NULL)
GO

ALTER TABLE dbo.Shifts ADD
    EnableNotifyExit bit NULL,
	NotifyEmployeeExitAt int NULL,
    EnableCompleteExit bit NULL,
	CompleteExitAt int NULL
GO

insert into NotificationMessageParameters(IDNotificationType,Scenario,Scope,Parameter,ParameterLanguageKey,NotificationLanguageKey)
values
	(72,1,'Body',1,'EmployeeName','Employee_Without_Exit'),
	(72,1,'Body',2,'BeginDate','Employee_Without_Exit'),
	(72,1,'Subject',0,'','Employee_Without_Exit')
GO

ALTER TABLE dbo.Notifications ADD
	AllowVTPortal int NULL
GO


CREATE VIEW [dbo].[sysrovwDashboardEmployeeStatus]
  AS
 	SELECT svEmployees.IdEmployee, 
	svEmployees.EmployeeName,
	Shifts.Name ShiftName,
	tmpLastPunch.Type as LastPunchType,
	tmpLastPunch.ShiftDate as LastPunchDate,
	tmpLastPunch.DateTime as LastPunchDateTime,
	tmpLastPunch.TypeData as LastPunchTypeData,
	Employees.Image EmployeeImage,
 	tmpAttendance.ShiftDate LastAttendanceDate, 
 	tmpAttendance.DateTime as LastAttendancePunchDatetime,
	ISNULL(Causes.Name, '') as LastAttendanceCause,
 	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttendanceStatus, 
 	tmpTasks.DateTime as LastTaskPunchDatetime, 
    ISNULL(Tasks.Project + ' : ' +  Tasks.Name,'') as LastTaskName,  
 	tmpcosts.DateTime as LastCostPunchDatetime, 
	CASE WHEN BusinessCentersPunch.Name IS NOT NULL THEN BusinessCentersPunch.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostCenterName,
	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE tmpLastPunch.LocationZone END As LocationName,
	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN '' ELSE Zones.Name END As ZoneName,
	CASE Shifts.ShiftType WHEN 2 THEN 1 ELSE 0 END AS InHolidays,
	CASE WHEN ProgrammedAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InAbsence,
	CASE WHEN ProgrammedAbsences.IDCause > 0 THEN Causes1.Name ELSE '' END AS InAbsenceCause,
	CASE WHEN tmpHourAbsences.IDCause > 0 THEN 1 ELSE 0 END AS InHourAbsence,
	CASE WHEN tmpHourAbsences.IDCause > 0 THEN Causes3.Name ELSE '' END AS InHourAbsenceCause,
	CASE WHEN tmpHoursHolidays.IDEmployee IS NOT NULL THEN 1 ELSE 0 END AS InHoursHolidays,
	CASE WHEN Requests.IDCause > 0 THEN 1 ELSE 0 END AS AbsenceRequested,
	CASE WHEN Requests.IDCause > 0 THEN Causes2.Name ELSE '' END AS AbsenceRequestCause,
	CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN 1 ELSE 0 END AS HoursAbsenceRequested,
	CASE WHEN tmpHourAbsenceRequest.IDCause > 0 THEN Causes2.Name ELSE '' END AS HoursAbsenceRequestCause
 	FROM sysrovwcurrentemployeegroups svEmployees WITH (NOLOCK)
	INNER JOIN Employees WITH (NOLOCK) ON Employees.Id = svEmployees.IDEmployee
	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberPunch', * FROM Punches WITH (NOLOCK) WHERE DATEDIFF(day,ShiftDate, GETDATE()) < 30  ) tmpLastPunch ON svEmployees.IDEmployee = tmpLastPunch.IdEmployee
	LEFT JOIN Zones ON tmpLastPunch.IDZone = ZOnes.ID
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WITH (NOLOCK) WHERE (ActualType = 1 or ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 30 ) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
	LEFT JOIN Causes ON tmpAttendance.TypeData = Causes.ID AND Causes.ID >0
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WITH (NOLOCK) WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
 	LEFT JOIN Tasks ON Tasks.Id = tmpTasks.TypeData
	LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
 	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WITH (NOLOCK) WHERE ActualType = 13 AND DATEDIFF(day,ShiftDate, GETDATE()) < 30) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
 	LEFT JOIN BusinessCenters BusinessCentersPunch WITH (NOLOCK) ON tmpCosts.TypeData = BusinessCentersPunch.ID
	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
	LEFT JOIN ProgrammedAbsences WITH (NOLOCK) ON ProgrammedAbsences.IDEmployee = svEmployees.IdEmployee AND  DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN ProgrammedAbsences.BeginDate AND ISNULL(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate))
	LEFT JOIN Causes as Causes1 WITH (NOLOCK) ON Causes1.ID = ProgrammedAbsences.IDCause
	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourHol', * FROM ProgrammedHolidays WITH (NOLOCK)) tmpHoursHolidays ON tmpHoursHolidays.Date = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) and tmpHoursHolidays.IDEmployee = svEmployees.IdEmployee
	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date ORDER BY idemployee ASC, Date DESC) AS 'RowNumberHourAbsence', * FROM ProgrammedCauses WITH (NOLOCK)) tmpHourAbsences ON  tmpHourAbsences.IDEmployee = svEmployees.IdEmployee AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsences.Date AND tmpHourAbsences.FinishDate
	LEFT JOIN Causes as Causes3 WITH (NOLOCK) ON Causes3.ID = tmpHourAbsences.IDCause
	LEFT JOIN Requests WITH (NOLOCK) ON Requests.IDEmployee = svEmployees.IdEmployee AND Requests.RequestType = 7 AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN Requests.Date1 AND Requests.Date2 AND Requests.Status IN (0,1)
	LEFT JOIN Causes as Causes2 WITH (NOLOCK) ON Causes2.ID = Requests.IDCause
	LEFT JOIN (SELECT row_number() OVER (PARTITION BY IdEmployee, Date1, Date2 ORDER BY idemployee ASC, RequestDate DESC) AS 'RowHourAbsenceRequest', * FROM Requests WITH (NOLOCK) WHERE RequestType = 9 AND Status IN (0,1)) tmpHourAbsenceRequest ON tmpHourAbsenceRequest.IDEmployee = svEmployees.IdEmployee  AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) BETWEEN tmpHourAbsenceRequest.Date1 AND tmpHourAbsenceRequest.Date2 
	LEFT JOIN Causes as Causes4 WITH (NOLOCK) ON Causes4.ID = tmpHourAbsenceRequest.IDCause
	WHERE CurrentEmployee = 1
 	AND (RowNumberAtt = 1 OR RowNumberAtt IS NULL)
	AND (RowNumberPunch = 1 OR RowNumberPunch IS NULL)
 	AND (RowNumberTsk = 1 OR RowNumberTsk IS NULL)
 	AND (RowNumberCost = 1 OR RowNumberCost IS NULL)
	AND (RowNumberHourHol = 1 OR RowNumberHourHol IS NULL)
    AND (RowNumberHourAbsence = 1 OR RowNumberHourAbsence IS NULL)
	AND (RowHourAbsenceRequest = 1 OR RowHourAbsenceRequest IS NULL)
GO



ALTER VIEW [dbo].[sysrovwEmployeeStatus]
  AS
	SELECT svEmployees.IdEmployee, 
	Employees.Image as EmployeeImage,
	Employees.Name as EmployeeName,
 	tmpAttendance.ShiftDate AttShiftDate, 
 	tmpAttendance.DateTime as AttDatetime, 
 	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttStatus, 
 	tmpTasks.DateTime as TskDatetime, 
    Tasks.Project + ' : ' +  Tasks.Name as TskName,  
	CASE WHEN BusinessCenters.Name IS NOT NULL THEN tmpcosts.DateTime ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN DailySchedule.Date ELSE CASE WHEN [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) IS NOT NULL THEN svEmployees.BeginDate END END) END AS CostDatetime ,
 	CASE WHEN BusinessCenters.Name IS NOT NULL THEN BusinessCenters.Name ELSE (CASE WHEN BusinessCentersShifts.Name IS NOT NULL THEN BusinessCentersShifts.Name ELSE [dbo].[GetEmployeeBusinessCenterNameOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostCenterName ,
	CASE WHEN BusinessCenters.ID IS NOT NULL THEN BusinessCenters.ID ELSE (CASE WHEN BusinessCentersShifts.ID IS NOT NULL THEN BusinessCentersShifts.ID ELSE [dbo].[GetEmployeeBusinessCenterOnDate](svEmployees.IDEmployee,GETDATE()) END) END AS CostIDCenter
 	FROM sysrovwcurrentemployeegroups svEmployees
	inner join Employees ON Employees.ID = svEmployees.IDEmployee
 	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE (ActualType = 1 OR ActualType = 2) AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
 	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4 AND DATEDIFF(day,ShiftDate, GETDATE()) < 120) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
 	left join Tasks ON Tasks.Id = tmpTasks.TypeData
	LEFT JOIN sysroLiveAdvancedParameters ON (ParameterName = 'LastPunchCenterEnabled' and convert(nvarchar,Value) = 1) 
 	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee AND (sysroLiveAdvancedParameters.Value = 1 OR tmpCosts.ShiftDate = DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0))
 	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID
	LEFT JOIN DailySchedule WITH (NOLOCK) ON DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) = DailySchedule.Date AND svEmployees.IdEmployee = DailySchedule.IDEmployee
	LEFT JOIN Shifts WITH (NOLOCK) ON Shifts.Id = DailySchedule.IDShift1
	LEFT JOIN BusinessCenters BusinessCentersShifts WITH (NOLOCK) ON Shifts.IDCenter = BusinessCentersShifts.Id
 	WHERE CurrentEmployee = 1
 	and (RowNumberAtt = 1 or RowNumberAtt is null)
 	and (RowNumberTsk = 1 or RowNumberTsk is null)
 	and (RowNumberCost = 1 or RowNumberCost is null)
 	and (tmpAttendance.DateTime IS NOT NULL OR tmpTasks.DateTime IS NOT NULL OR tmpCosts.DateTime IS NOT NULL)
GO

 ALTER PROCEDURE [dbo].[Analytics_Concepts]
      	@initialDate smalldatetime,
     	@endDate smalldatetime,
     	@idpassport int,
     	@employeeFilter nvarchar(max),
     	@conceptsFilter nvarchar(max),
     	@userFieldsFilter nvarchar(max),
     	@includeZeros tinyint
      AS
      DECLARE @employeeIDs Table(idEmployee int)
      DECLARE @conceptIDs Table(idConcept int)
  	  DECLARE @pinitialDate smalldatetime = @initialDate,
     	@pendDate smalldatetime = @endDate,
     	@pidpassport int = @idpassport,
     	@pemployeeFilter nvarchar(max) = @employeeFilter,
     	@pconceptsFilter nvarchar(max) = @conceptsFilter,
     	@puserFieldsFilter nvarchar(max) = @userFieldsFilter,
     	@pincludeZeros tinyint = @includeZeros

		DECLARE @intdatefirst int
		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
		SET DateFirst @intdatefirst;

      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
      insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');
      IF @pincludeZeros = 1 
     	 BEGIN
     		 WITH alldays AS (
     		  SELECT @initialDate AS dt
     		  UNION ALL
     		  SELECT DATEADD(dd, 1, dt)
     			FROM alldays s
     		   WHERE DATEADD(dd, 1, dt) <= @pendDate)
     	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
     			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
     			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
     			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
     			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
     			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
     			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
     			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10
     	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
     				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
     				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
     				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
     			) reqReg
     			INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
     			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
     			LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
     	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
     			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
     			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
     			reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) 
     			+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 
     			10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
     			dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
     	option (maxrecursion 0)
      END
      ELSE
     	 BEGIN
     		WITH alldays AS (
     		  SELECT @initialDate AS dt
     		  UNION ALL
     		  SELECT DATEADD(dd, 1, dt)
     			FROM alldays s
     		   WHERE DATEADD(dd, 1, dt) <= @pendDate)
     	SELECT	CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 10) AS KeyView, 
     			reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept, 
     			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName, 
     			reqReg.conceptName AS ConceptName, reqReg.dt AS Date, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes, 
     			YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, 
     			reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, dbo.sysroEmployeeGroups.Path, 
     			dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
     			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @pendDate AS EndPeriod,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
     			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,
     			dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10
     	FROM	(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays, 
     				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
     				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
     				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
     			) reqReg
     			INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate
     			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
     			INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept
     	GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
     			dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
     			YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, reqReg.dt), DATEPART(dy, 
     			reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar) 
     			+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120), 
     			10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
     			dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
     	option (maxrecursion 0)
      END
GO

 ALTER PROCEDURE [dbo].[Analytics_CostCenters]  
        @initialDate smalldatetime,  
        @endDate smalldatetime,  
        @idpassport int,  
        @userFieldsFilter nvarchar(max),  
     @businessCenterFilter nvarchar(max)  
        AS  
     DECLARE @businessCenterIDs Table(idBusinessCenter int)  
      declare @pinitialDate smalldatetime = @initialDate,    
       @pendDate smalldatetime = @endDate,    
       @pidpassport int = @idpassport,    
       @pbusinessCenterFilter nvarchar(max) = @businessCenterFilter,    
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
	   	DECLARE @intdatefirst int
		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
		SET DateFirst @intdatefirst;
     insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
         SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                               + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date,  
                               dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
             isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
             isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                               (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
                               dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter ,                              
             dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
             dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP  
         FROM         dbo.sysroEmployeeGroups with (nolock)   
                               INNER JOIN dbo.Causes with (nolock)   
                               INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo
  .sysroEmployeeGroups.EndDate   
             INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
             INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
             LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
             LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON isnull(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
          where     isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
 		 and isnull(dbo.DailyCauses.IDCenter,0) in (select idcenter from sysroPassports_Centers where IDPassport = (select idparentpassport from sysroPassports where id = @pidpassport))
       and dbo.dailycauses.date between @pinitialDate and @pendDate 
GO

 ALTER PROCEDURE [dbo].[Analytics_Incidences]  
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
        SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                              + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                              dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,   
                              dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año,   
                              (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy,   
                              dbo.DailySchedule.Date) AS DayOfYear, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,   
                              dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,   
                              dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,  
             dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod, dbo.DailySchedule.IDAssignment,  
          case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,  
                             (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,  
                             (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,  
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
             dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailySchedule.Date) As UserField1,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailySchedule.Date) As UserField2,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailySchedule.Date) As UserField3,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailySchedule.Date) As UserField4,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailySchedule.Date) As UserField5,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailySchedule.Date) As UserField6,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailySchedule.Date) As UserField7,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailySchedule.Date) As UserField8,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailySchedule.Date) As UserField9,  
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailySchedule.Date) As UserField10,  
          convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark  
        FROM         dbo.sysroEmployeeGroups with (nolock)  
            INNER JOIN dbo.Causes with (nolock)  
            INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause   
            INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate  
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate   
            LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)  
            INNER JOIN dbo.DailyIncidences with (nolock)  
            INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate  
            LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID   
            LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID  
            LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
         LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks  
       WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1  
          AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)   
         and dbo.dailyCauses.Date between @pinitialDate and @pendDate  
        GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,   
                              YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                  + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()   
                              THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(wk,   
                              dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,   
                              dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,   
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text)
GO

  ALTER PROCEDURE  [dbo].[Analytics_Schedule]
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
   		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
   		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
   		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
   		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
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
 		(SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate
       FROM         dbo.sysroEmployeesShifts with (nolock) 
   		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
   		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
   		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
   	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
   		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
   	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
   		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
   		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
   		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
   		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
   		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
   		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
   		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark)
GO




ALTER TABLE sysroPassports add ShowHelp int null default(0)
GO

update sysropassports set showhelp=0 where showhelp is null
GO

CREATE PROCEDURE [dbo].[RemoveInactivePassportPermissions]  AS  
    BEGIN  

	delete from sysroPermissionsOverGroups where PassportID in(
	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
	)

	delete from sysroPermissionsOverFeatures where PassportID in(
	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
	)

	delete from sysroPermissionsOverEmployeesExceptions where PassportID in(
	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
	)

	delete from sysroPermissionsOverRequests where IDParentPassport in(
	select IDParentPassport from sysroPassports where GroupType<> 'U' AND NOT ((StartDate IS NULL OR StartDate <= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND (ExpirationDate IS NULL OR ExpirationDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) AND isnull(State,0) = 1)
	)
	END
GO


ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
	(
    @IDAction int,
    @IDObject int,@Version int
	)
	AS
	/* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
   	BEGIN
 		DECLARE @pIDAction int = @IDAction,  
			@pIDObject int = @IDObject, @pVersion int = @Version; 
   		IF @pIDAction = -2
   		BEGIN
  			exec dbo.sysro_GenerateAllPermissionsOverGroups @pVersion
  			exec dbo.sysro_GenerateAllPermissionsOverFeatures
   			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
  			exec dbo.GenerateAllRequestPassportPermission
   		END
  		IF @pIDAction = -1 -- Cambio de dia
   		BEGIN
  			exec dbo.GenerateChangeDayRequestPassportPermission
   		END
   		IF @pIDAction = 0 -- Creación passport
   		BEGIN
  			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
  			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
  			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
			exec dbo.InsertPassportRequestsPermission @pIDObject
   		END
   		IF @pIDAction = 1 -- Modificación passport
   		BEGIN
  			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
  			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
  			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
			exec dbo.AlterPassportRequestsPermission @pIDObject
   		END
   		IF @pIDAction = 2 -- Creación solicitud
   		BEGIN
   			exec dbo.InsertRequestPassportPermission @pIDObject
   		END
   		IF @pIDAction = 3 -- Creación grupo de empleados
   		BEGIN
   			exec dbo.InsertGroupPassportPermission @pIDObject,@pVersion
   		END

		exec dbo.RemoveInactivePassportPermissions
   	END	
GO

UPDATE sysroParameters SET Data='507' WHERE ID='DBVersion'
GO

