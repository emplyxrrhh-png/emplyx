
ALTER VIEW [dbo].[sysroScheduleCube1] AS
SELECT     RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                      + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.DailyAccruals.IDConcept, 
                      dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, 
                      SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, YEAR(dbo.DailyAccruals.Date) AS Año, (DATEPART(dw, 
                      dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyAccruals.Date) AS WeekOfYear, DATEPART(dy, 
                      dbo.DailyAccruals.Date) AS DayOfYear
FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                      dbo.DailyAccruals ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.DailyAccruals.IDEmployee INNER JOIN
                      dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name, dbo.DailyAccruals.Date, 
                      RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                      + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10), dbo.DailyAccruals.IDEmployee, dbo.DailyAccruals.IDConcept, MONTH(dbo.DailyAccruals.Date), 
                      YEAR(dbo.DailyAccruals.Date), (DATEPART(dw, dbo.DailyAccruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, dbo.DailyAccruals.Date), DATEPART(dy, 
                      dbo.DailyAccruals.Date)
GO


ALTER VIEW [dbo].[sysroScheduleCube2] AS
SELECT     RIGHT('0000000' + CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar), 7) + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) 
                      AS KeyView, dbo.sysroEmployeesShifts.IDEmployee, dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, 
                      dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) 
                      AS Count, MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
                      dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
                      DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear
FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                      dbo.sysroEmployeesShifts ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.sysroEmployeesShifts.IDEmployee
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
                      dbo.sysroEmployeesShifts.CurrentDate, dbo.sysroEmployeesShifts.IDEmployee, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
                      YEAR(dbo.sysroEmployeesShifts.CurrentDate), DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), 
                      (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1

GO




ALTER VIEW [dbo].[sysroScheduleCube3] AS
SELECT     CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.DailySchedule.IDEmployee, 
                      dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName, dbo.TimeZones.Name AS ZoneTime, 
                      dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, YEAR(dbo.DailySchedule.Date) AS Año, (DATEPART(dw, dbo.DailySchedule.Date) 
                      + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
                      CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName, dbo.Employees.Name AS EmployeeName, 
                      dbo.Groups.Name AS GroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Causes INNER JOIN
                      dbo.DailyCauses ON dbo.Causes.ID = dbo.DailyCauses.IDCause INNER JOIN
                      dbo.DailySchedule ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON 
                      dbo.EmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.EmployeeGroups.BeginDate <= dbo.DailySchedule.Date AND 
                      dbo.EmployeeGroups.EndDate >= dbo.DailySchedule.Date INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN
                      dbo.sysroDailyIncidencesDescription INNER JOIN
                      dbo.DailyIncidences INNER JOIN
                      dbo.TimeZones ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON 
                      dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND 
                      dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID LEFT OUTER JOIN
                      dbo.Shifts ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID LEFT OUTER JOIN
                      dbo.Shifts AS Shifts_1 ON dbo.DailySchedule.IDShift1 = Shifts_1.ID
GROUP BY dbo.DailySchedule.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name, 
                      YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.DailySchedule.IDEmployee, 120) + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), 
                      CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.Groups.Name, DATEPART(wk, 
                      dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1
GO

ALTER FUNCTION GetRequestNextLevelPassports
 (	
 	@idRequest int	
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
 	DECLARE @RetNames nvarchar(1000), 
 			@PassportName nvarchar(50)
 	SET @RetNames = ''
 	DECLARE PassportsCursor CURSOR
 		FOR SELECT sysroPassports.Name 
 			FROM sysroPassports INNER JOIN sysroPassports Parents ON sysroPassports.IDParentPassport = Parents.ID 
 			WHERE sysroPassports.GroupType <> 'U' AND 
 				  Parents.LevelOfAuthority IS NOT NULL AND 
 				  Parents.LevelOfAuthority = dbo.GetRequestNextLevel(@idRequest) AND 
 				  dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 0
 			ORDER BY sysroPassports.Name
 	OPEN PassportsCursor
 	FETCH NEXT FROM PassportsCursor
 	INTO @PassportName
 	WHILE @@FETCH_STATUS = 0
 	BEGIN	
 	
 		IF @RetNames = ''
 			BEGIN
 				SET @RetNames = @PassportName
 			END
 		ELSE
 			BEGIN
 				SET @RetNames = @RetNames + ', '+ @PassportName
 			END
 		FETCH NEXT FROM PassportsCursor 
 		INTO @PassportName
 	END 
 	CLOSE PassportsCursor
 	DEALLOCATE PassportsCursor
 	RETURN @RetNames
 END

GO

  ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
  	(
  		@idPassport int,
 		@idRequest int
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @LevelsBelow int
  	DECLARE @LevelOfAuthority int
 	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
 	FROM sysroPassports 
 	WHERE sysroPassports.ID = @idPassport
 	DECLARE @featureAlias nvarchar(100),
 			@EmployeefeatureID int,
  			@idEmployee int,
 			@RequestLevel int
  	
 	SELECT @featureAlias = CASE Requests.RequestType 
 								WHEN 1 THEN 'Employees.UserFields.Requests' 
 								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
 								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
 								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
 								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
 								WHEN 6 THEN 'Calendar.Requests.Vacations'
 								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
 								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
 								ELSE 'Calendar.Requests.ShiftExchange' END,
 		   @EmployeefeatureID = CASE Requests.RequestType
 								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
 								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
 								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
 		   @idEmployee = Requests.IDEmployee,
 		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
 	FROM Requests
 	WHERE Requests.ID = @idRequest
 	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
 	SELECT @LevelsBelow = 
 	(SELECT COUNT( DISTINCT Parents.LevelOfAuthority)
 	 FROM sysroPassports INNER JOIN sysroPassports Parents
 			ON sysroPassports.IDParentPassport = Parents.ID
 	 WHERE sysroPassports.GroupType <> 'U' AND
 	 	  Parents.LevelOfAuthority IS NOT NULL AND
 	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
 		  Parents.LevelOfAuthority <= @RequestLevel AND
 		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
 		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
 	 )
 	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
  	
  RETURN @LevelsBelow
  END

GO


ALTER FUNCTION [dbo].[GetRequestMinStatusLevel]
  	(
  		@idPassport int,
 		@featureAlias nvarchar(100),
 		@EmployeefeatureID int,
  		@idEmployee int
  	)
  RETURNS int
  AS
  BEGIN
  	/* While looking only at permissions defined directly on passport,
  	returns the first permission found in the employees groups hierarchy */
  	DECLARE @MinStatusLevel int
  	DECLARE @LevelOfAuthority int
 	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
 	FROM sysroPassports 
 	WHERE sysroPassports.ID = @idPassport
  	
 	SELECT @MinStatusLevel = 
 	(SELECT TOP 1 Parents.LevelOfAuthority
 	 FROM sysroPassports INNER JOIN sysroPassports Parents
 			ON sysroPassports.IDParentPassport = Parents.ID
 	 WHERE sysroPassports.GroupType <> 'U' AND
 	 	  Parents.LevelOfAuthority IS NOT NULL AND
 	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
 		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
 		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
 	 ORDER BY Parents.LevelOfAuthority ASC)
  	
  RETURN @MinStatusLevel
  END

GO


 ALTER FUNCTION [dbo].[GetRequestNextLevel]
 (	
 	@idRequest int	
 )
 RETURNS int
 AS
 BEGIN
 	DECLARE @NextLevel int
 	SELECT @NextLevel = 
 	(SELECT MAX(Parents.LevelOfAuthority)
 	FROM sysroPassports INNER JOIN sysroPassports Parents ON sysroPassports.IDParentPassport = Parents.ID 
 	WHERE sysroPassports.GroupType <> 'U' AND 
 	      Parents.LevelOfAuthority IS NOT NULL AND 
 	      Parents.LevelOfAuthority < ISNULL(Requests.StatusLevel, 11) AND 
 	      dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 0)
 	FROM Requests
 	WHERE Requests.ID = @idRequest
 		   
 	RETURN @NextLevel
 END

GO

UPDATE sysroPassports SET LevelOfAuthority = 1
WHERE (GroupType = 'U') AND (IDParentPassport IS NULL) AND (LevelOfAuthority IS NULL OR LevelOfAuthority <= 0)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='300' WHERE ID='DBVersion'
GO
