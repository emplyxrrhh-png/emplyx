
UPDATE sysroGUI SET Priority = 615
WHERE (IDPath = N'Portal\ShiftControl\Analytics') AND (Priority = 675)
GO

ALTER TABLE Groups ALTER COLUMN DescriptionGroup NVARCHAR(4000)
GO

ALTER TABLE sysRopassports ALTER COLUMN Description NVARCHAR(4000)
GO

-- NUEVA TABLA PARA GUARDAR LAS VISTAS DEL CUBO DE CALENDARIO
CREATE TABLE [dbo].[sysroSchedulerViews] (
	[ID] [int] NOT NULL,
	[IdView] [int] NOT NULL,
	[IdPassport] [int] NOT NULL,
	[NameView] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL DEFAULT (''),
	[DateView] [datetime] NOT NULL,
	[Employees] [nvarchar](4000) NULL DEFAULT (''),
	[DateInf] [datetime] NOT NULL,
	[DateSup] [datetime] NOT NULL,
	[CubeLayout] [nvarchar](MAX) NULL DEFAULT ('') ,
 CONSTRAINT [PK_sysroSchedulerViews] PRIMARY KEY NONCLUSTERED 								
(								
	[ID] ASC							
) ON [PRIMARY]								
) ON [PRIMARY]								
GO

ALTER VIEW [dbo].[sysroScheduleCube1] AS
SELECT     RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                       + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10) AS KeyView, dbo.DailyAccruals.IDEmployee, dbo.sysrovwAllEmployeeGroups.GroupName, 
                       dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name AS ConceptName, dbo.DailyAccruals.Date, SUM(dbo.DailyAccruals.Value) AS Value, COUNT(*) 
                       AS Count, MONTH(dbo.DailyAccruals.Date) AS Mes, YEAR(dbo.DailyAccruals.Date) AS Año,
DATEPART(dw, dbo.DailyAccruals.Date) AS DayOfWeek, 
DATEPART(wk, dbo.DailyAccruals.Date) AS WeekOfYear,  
DATEPART(dy, dbo.DailyAccruals.Date) AS DayOfYear
FROM         dbo.sysrovwAllEmployeeGroups INNER JOIN
                       dbo.DailyAccruals ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.DailyAccruals.IDEmployee INNER JOIN
                       dbo.Concepts ON dbo.DailyAccruals.IDConcept = dbo.Concepts.ID
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.Concepts.Name, dbo.DailyAccruals.Date, 
                       RIGHT('0000000' + CAST(dbo.DailyAccruals.IDEmployee AS varchar), 7) + '-' + RIGHT('00000' + CAST(dbo.DailyAccruals.IDConcept AS varchar), 5) 
                       + '-' + + LEFT(CONVERT(VARCHAR, dbo.DailyAccruals.Date, 120), 10), dbo.DailyAccruals.IDEmployee, MONTH(dbo.DailyAccruals.Date), YEAR(dbo.DailyAccruals.Date)
GO

ALTER VIEW [dbo].[sysroScheduleCube2] AS
SELECT     RIGHT('0000000' + CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar), 7) + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
dbo.sysroEmployeesShifts.IDEmployee, dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, 
dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours, COUNT(*) AS Count, 
MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año,
DATEPART(dw,  dbo.sysroEmployeesShifts.CurrentDate) AS DayOfWeek, 
DATEPART(wk,  dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear,  
DATEPART(dy,  dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear
FROM dbo.sysrovwAllEmployeeGroups INNER JOIN
dbo.sysroEmployeesShifts ON dbo.sysrovwAllEmployeeGroups.IDEmployee = dbo.sysroEmployeesShifts.IDEmployee
GROUP BY dbo.sysrovwAllEmployeeGroups.GroupName, dbo.sysrovwAllEmployeeGroups.EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
dbo.sysroEmployeesShifts.CurrentDate, dbo.sysroEmployeesShifts.IDEmployee, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
YEAR(dbo.sysroEmployeesShifts.CurrentDate)
GO

ALTER VIEW [dbo].[sysroScheduleCube3] AS
 SELECT rank() OVER (ORDER BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeesShifts.GroupName, dbo.sysroEmployeesShifts.Name, dbo.sysroEmployeesShifts.CurrentDate, dbo.sysroEmployeesShifts.ShiftName, 
                       dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name) as KeyView,
 sysroEmployeesShifts.IDEmployee, sysroEmployeesShifts.GroupName, sysroEmployeesShifts.Name AS EmployeeName, 
                       sysroEmployeesShifts.CurrentDate AS Date, sysroEmployeesShifts.ShiftName, sysroDailyIncidencesDescription.Description AS IncidenceName, 
                       TimeZones.Name AS ZoneTime, Causes.Name AS CauseName, SUM(DailyCauses.Value) AS Value, COUNT(*) AS Count, MONTH(sysroEmployeesShifts.CurrentDate) 
                       AS Mes, YEAR(sysroEmployeesShifts.CurrentDate) AS Año,
DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfWeek, 
DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear,  
DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear

 FROM         sysroEmployeesShifts INNER JOIN
                       DailyCauses ON sysroEmployeesShifts.IDEmployee = DailyCauses.IDEmployee AND sysroEmployeesShifts.CurrentDate = DailyCauses.Date INNER JOIN
                       Causes ON DailyCauses.IDCause = Causes.ID LEFT OUTER JOIN
                       DailyIncidences INNER JOIN
                       TimeZones ON DailyIncidences.IDZone = TimeZones.ID ON DailyCauses.IDEmployee = DailyIncidences.IDEmployee AND 
                       DailyCauses.Date = DailyIncidences.Date AND DailyCauses.IDRelatedIncidence = DailyIncidences.ID INNER JOIN
                       sysroDailyIncidencesDescription ON DailyIncidences.IDType = sysroDailyIncidencesDescription.IDIncidence
 GROUP BY sysroEmployeesShifts.IDEmployee, sysroEmployeesShifts.GroupName, sysroEmployeesShifts.Name, sysroEmployeesShifts.CurrentDate, 
                       sysroEmployeesShifts.ShiftName, sysroDailyIncidencesDescription.Description, TimeZones.Name, Causes.Name, MONTH(sysroEmployeesShifts.CurrentDate), 
                       YEAR(sysroEmployeesShifts.CurrentDate)
GO

-- Vista para el analizador de impactos de actualización de Win32 a Live
CREATE VIEW [dbo].[sysrovwCurrentEmployeeConceptAnnualLimits]
AS
SELECT * FROM EmployeeConceptAnnualLimits WHERE IDYear=year(GETDATE())
GO

--Terminal mx8
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,41,'TA','1','Blind','E,S,X','LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,42,'TA','1','Fast',NULL,'LocalServer,ServerLocal,Server,Local','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,43,'TA','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,44,'TATSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,45,'TSK','1','Interactive',NULL,'ServerLocal,Server','1,0','0','1,0','0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',2,46,'',NULL,NULL,NULL,NULL,'0',NULL,NULL,NULL,NULL)
GO

ALTER PROCEDURE [dbo].[WebLogin_Passports_Update] 
(@id int,
@idParentPassport int,
@groupType varchar(1),
@name nvarchar(50),
@description nvarchar(4000),
@email nvarchar(100),
@idUser int,
@idEmployee int,
@idLanguage int,
@levelOfAuthority tinyint,
@ConfData text,
@AuthenticationMerge nvarchar(50),
@StartDate smalldatetime,
@ExpirationDate smalldatetime,
@State smallint)
AS
IF @groupType <> 'U' 
	BEGIN
		SET @levelOfAuthority = NULL
	END
UPDATE sysroPassports SET
	IDParentPassport = @idParentPassport,
	GroupType = @groupType,
	Name = @name,
	Description = @description,
	Email = @email,
	IDUser = @idUser,
	IDEmployee = @idEmployee,
	IDLanguage = @idLanguage,
	LevelOfAuthority = @levelOfAuthority,
	ConfData = @ConfData,
	AuthenticationMerge = @AuthenticationMerge,
	StartDate = @StartDate,
	ExpirationDate = @ExpirationDate,
	[State] = @State
WHERE ID = @id

RETURN
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='291' WHERE ID='DBVersion'
GO
