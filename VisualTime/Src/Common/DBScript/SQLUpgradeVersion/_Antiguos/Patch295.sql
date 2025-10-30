--- Añadimos el modulo de kpi's y los datos iniciales para estos
INSERT INTO [dbo].[sysroGUI]
    ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
VALUES
    ('Portal\ShiftControl\KPI','KPI','Indicators/Indicators.aspx','Indicators.png',NULL,NULL,'Feature\KPIs',NULL,670,NULL,'U:KPI.Definition=Read')
GO

INSERT INTO [dbo].[sysroFeatures]
    ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES
    (27000,NULL,'KPI','Indicadores','','U','RWA',NULL)
GO

INSERT INTO [dbo].[sysroFeatures]
    ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES
    (27100,27000,'KPI.Definition','Definición','','U','RWA',NULL)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures]
	([IDPassport],[IDFeature],[Permission])
VALUES
   (3,27000,9)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures]
	([IDPassport],[IDFeature],[Permission])
VALUES
   (3,27100,9)
GO

CREATE TABLE [dbo].[Indicators](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[IDFirstConcept] [int] NOT NULL,
	[IDSecondConcept] [int] NOT NULL,
	[LimitValue] [numeric](10, 2) NULL,
	[DesiredValue] [numeric](10, 2) NULL,
	[Condition] [text] NULL,
	[AllowNotification] [bit] NULL DEFAULT(0),
	[TypeNotification] [smallint] NULL DEFAULT(0),
	[Description] [nvarchar](4000) NULL,


 CONSTRAINT [PK_Indicators] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO


CREATE TABLE [dbo].[GroupIndicators](
	[IDGroup] [int] NOT NULL,
	[IDIndicator] [int] NOT NULL,
 CONSTRAINT [PK_GroupIndicators] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[IDIndicator] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE TABLE [dbo].[sysroInidcatorTypes](
	[ID] [tinyint] NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_sysroInidcatorTypes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[sysroInidcatorTypes] 
	([id], [Name]) 
VALUES
	(1,'ATTENDANCE')
GO

CREATE TABLE [dbo].[TMPIndicatorsAnalysis](
	[IDGroup] [int] NOT NULL,
	[IDIndicator] [int] NOT NULL,
	[MONTH_A] [numeric](10, 2) NULL,
	[MONTH_1] [numeric](10, 2) NULL,
	[MONTH_2] [numeric](10, 2) NULL,
	[Q1] [numeric](10, 2) NULL,
	[Q2] [numeric](10, 2) NULL,
	[Q3] [numeric](10, 2) NULL,
	[Q4] [numeric](10, 2) NULL,
	[Year_A] [numeric](10, 2) NULL,
	[Year_1] [numeric](10, 2) NULL,
	

 CONSTRAINT [PK_TMPIndicatorsAnalysis] PRIMARY KEY CLUSTERED 
(
	[IDGroup] ASC,
	[IDIndicator] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE TABLE [dbo].[TMPGroupIndicatorsAnalysis](
	[IDGroup] [int] NOT NULL,
	[IDGroupParent] [int] NOT NULL,
	[IDIndicator] [int] NOT NULL,
	[GroupPath] [nvarchar](64) NULL,
	[GroupValue][numeric](10, 2) NULL,
	[GroupPercentage][numeric](10, 2) NULL,
	[MONTH_1] [numeric](10, 2) NULL,
	[MONTH_1_Percentage][numeric](10, 2) NULL,
	[MONTH_2] [numeric](10, 2) NULL,
	[MONTH_2_Percentage][numeric](10, 2) NULL,
	[Q1] [numeric](10, 2) NULL,
	[Q1_Percentage][numeric](10, 2) NULL,
	[Q2] [numeric](10, 2) NULL,
	[Q2_Percentage][numeric](10, 2) NULL,
	[Q3] [numeric](10, 2) NULL,
	[Q3_Percentage][numeric](10, 2) NULL,
	[Q4] [numeric](10, 2) NULL,
	[Q4_Percentage][numeric](10, 2) NULL,
	[Year_A] [numeric](10, 2) NULL,
	[Year_A_Percentage][numeric](10, 2) NULL,
	[Year_1] [numeric](10, 2) NULL,
	[Year_1_Percentage][numeric](10, 2) NULL,
	
 CONSTRAINT [PK_TMPGroupIndicatorsAnalysis] PRIMARY KEY CLUSTERED 
(
	[IDGroupParent] ASC,
	[IDGroup] ASC,
	[IDIndicator] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO
--- Fin Modulo de kpi's 

--- Añadimos importación de empresas
INSERT INTO [dbo].[ImportGuides]
       ([ID],[Name],[Template],[Mode],[Type],[FormatFilePath],[SourceFilePath],[Separator],[CopySource],[LastLog])
 VALUES
       (4,'Carga de empresas',0,1,1,'','','',0,NULL)
GO
--- Fin de importación de empresas


ALTER TABLE [dbo].[sysroSchedulerViews] ADD TypeView [nvarchar](1) default ''
GO

UPDATE  [dbo].[sysroSchedulerViews] SET TypeView = 'S'
GO

ALTER TABLE [dbo].[sysroSchedulerViews] ADD FilterData [nvarchar](100) default ''
GO

UPDATE  [dbo].[sysroSchedulerViews] SET FilterData = ''
GO

--VISTA PARA LA PANTALLA DE FILTROS DE ACCESO POR MATRICULAS
CREATE VIEW [dbo].[sysroAccessPlates]
AS
SELECT     Punches.ID, Punches.IDEmployee, Punches.DateTime AS DatePunche, CONVERT(VARCHAR(8), Punches.DateTime, 108) AS TimePunche, CONVERT(NVARCHAR(4000), 
                      Punches.TypeDetails) AS TypeDetails, Employees.Name AS NameEmployee, Zones.Name AS NameZone
FROM         Punches INNER JOIN
                      TerminalReaders ON Punches.IDTerminal = TerminalReaders.IDTerminal AND Punches.IDReader = TerminalReaders.ID LEFT OUTER JOIN
                      Employees ON Punches.IDEmployee = Employees.ID LEFT OUTER JOIN
                      Zones ON Punches.IDZone = Zones.ID
WHERE     (TerminalReaders.Type = N'MAT') AND (NOT (Punches.TypeDetails IS NULL)) AND (Punches.Type = 5 OR Punches.Type = 6 OR Punches.Type = 7)
GO

CREATE FUNCTION GetAllEmployeeUserFieldValueEx
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

-- Terminal mx8
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8', 1, 48,'ACC', '0','Blind', 'X', 'Local', '0', '1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,49,'ACC','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,50,'ACCTA','0','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mx8',1,51,'ACCTA','1','Blind','X','Local','0','1,0','0','1,0','1,0')
GO
INSERT INTO [sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput])
VALUES ('mx8',1,52,'DIN',1,'Interactive','Server','1,0','0','0','0','0')
GO
-- Terminal rxC
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxC',1,1,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxC',1,2,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxC',1,3,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

-- Terminal rxCe
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCe',1,1,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCe',1,2,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCe',1,3,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='295' WHERE ID='DBVersion'
GO

