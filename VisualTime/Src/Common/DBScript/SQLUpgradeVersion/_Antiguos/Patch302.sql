-- Fallo en el script 212 y afectó a Union Industrial Papelera
ALTER TABLE DBO.Concepts ADD IsAccrualWork bit NULL
GO
UPDATE DBO.Concepts SET IsAccrualWork = 0
GO
UPDATE dbo.sysroGUI SET RequiredFunctionalities = 'E:Totals.Query=Read' WHERE IDPath = 'LivePortal\TimeControl\MyAccruals'
GO

-- Fallo en el script 295, se creaba la función sin el dbo
CREATE FUNCTION [dbo].[GetAllEmployeeUserFieldValueEx]
(	
  	@FieldName nvarchar(50),
  	@strDate nvarchar(20)
)
RETURNS @ValueTable table(idEmployee int, [value] varchar(4000), [Date] smalldatetime) 
AS
BEGIN
DECLARE @Date smalldatetime
SET @Date = CONVERT(SMALLDATETIME,@strDate,120)
INSERT INTO @ValueTable
SELECT Employees.ID, 
		(SELECT TOP 1 CONVERT(varchar(4000), [Value])
		 FROM EmployeeUserFieldValues
		 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
		  	   EmployeeUserFieldValues.FieldName = @FieldName AND
			   EmployeeUserFieldValues.Date <= @Date
		 ORDER BY EmployeeUserFieldValues.Date DESC),
		ISNULL((SELECT TOP 1 [Date]
			    FROM EmployeeUserFieldValues
		        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
		  	          EmployeeUserFieldValues.FieldName = @FieldName AND
			          EmployeeUserFieldValues.Date <= @Date
		        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
FROM Employees, sysroUserFields
WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND sysroUserFields.FieldName = @FieldName
RETURN
END
GO



ALTER TABLE sysroReaderTemplates ADD RequiredFeatures [nvarchar](200) default ''
GO

UPDATE  sysroReaderTemplates SET RequiredFeatures = ''
GO

UPDATE  sysroReaderTemplates SET RequiredFeatures = 'Version\LiveExpress' WHERE Type = 'rxC' or Type = 'rxF' or Type = 'rxCE'
GO


ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
	(
	@idPassport int,
	@idRequest int
	)
RETURNS int
AS
BEGIN
	DECLARE @LevelsBelow int,
			@LevelOfAuthority int,
   			@featureAlias nvarchar(100),
			@EmployeefeatureID int,
			@idEmployee int,
			@RequestLevel int
	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
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
		(SELECT COUNT( DISTINCT dbo.GetPassportLevelOfAuthority(Parents.ID))
		 FROM sysroPassports INNER JOIN sysroPassports Parents
		 ON sysroPassports.IDParentPassport = Parents.ID INNER JOIN 
		 (SELECT Id, dbo.GetPassportLevelOfAuthority(ID) AS value FROM sysroPassports) gpla
		 ON gpla.ID = Parents.id 
		 WHERE sysroPassports.GroupType <> 'U' AND gpla.value > @LevelOfAuthority AND gpla.value <= @RequestLevel AND
		 (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
		 (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0)
 
   	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
	
	RETURN @LevelsBelow
END
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='302' WHERE ID='DBVersion'
GO
