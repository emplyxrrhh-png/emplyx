--Creación de Tabla de sysroReportGroups

CREATE TABLE [dbo].[sysroReportGroups] (
	[ID] [smallint] IDENTITY (1, 1) NOT NULL ,
	[Name] [nvarchar] (128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroReportGroups] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroReportGroups] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroReportGroups] ADD 
	CONSTRAINT [IX_sysroReportGroups] UNIQUE  NONCLUSTERED 
	(
		[Name]
	)  ON [PRIMARY] 
GO


--Creación de Tabla de sysroReportGroupConcepts


CREATE TABLE [dbo].[sysroReportGroupConcepts] (
	[IDReportGroup] [smallint] NOT NULL ,
	[IDConcept] [smallint] NOT NULL ,
	[Position] [smallint] NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroReportGroupConcepts] ADD 
	CONSTRAINT [IX_sysroReportGroupConcepts] UNIQUE  NONCLUSTERED 
	(
		[IDReportGroup],
		[IDConcept]
	)  ON [PRIMARY] 
GO

-- Creamos tabla para gestión de usuarios y grupos monitorizados desde WebValidator
CREATE TABLE [dbo].[WebValidatorMonitoredEmployees] (
	[IDUser] [tinyint] NOT NULL ,
	[IDEmployee] [int] NULL ,
	[IDGroup] [int] NULL ,
	CONSTRAINT [IX_WebValidatorMonitoredEmployees] UNIQUE  NONCLUSTERED 
	(
		[IDUser],
		[IDEmployee],
		[IDGroup]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


-- Creación de las tablas para Auditoría

/****** Objeto: tabla [dbo].[sysroAuditActions]    fecha de la secuencia de comandos: 10/11/2005 15:03:52 ******/
CREATE TABLE [dbo].[sysroAuditActions] (
	[ID] [smallint] NOT NULL ,
	[Name] [varchar] (15) NOT NULL ,
	[Description] [varchar] (50) NULL 
) ON [PRIMARY]
GO

/****** Objeto: tabla [dbo].[sysroAuditElements]    fecha de la secuencia de comandos: 10/11/2005 15:03:53 ******/
CREATE TABLE [dbo].[sysroAuditElements] (
	[ID] [smallint] NOT NULL ,
	[Name] [varchar] (20) NOT NULL ,
	[Description] [varchar] (50) NULL 
) ON [PRIMARY]
GO


/****** Objeto: tabla [dbo].[sysroAuditScreens]    fecha de la secuencia de comandos: 10/11/2005 15:03:53 ******/
CREATE TABLE [dbo].[sysroAuditScreens] (
	[ID] [smallint] NOT NULL ,
	[Name] [varchar] (20) NOT NULL ,
	[Description] [varchar] (50) NULL 
) ON [PRIMARY]
GO

/****** Objeto: tabla [dbo].[Audit]    fecha de la secuencia de comandos: 10/11/2005 15:03:53 ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Audit]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Audit]
GO

CREATE TABLE [dbo].[Audit] (
	[ID] [numeric](18, 0) IDENTITY (1, 1) NOT NULL ,
	[IDuser] [int] NOT NULL ,
	[Date] [datetime] NOT NULL ,
	[ActionID] [smallint] NOT NULL ,
	[ScreenID] [smallint] NOT NULL ,
	[ElementID] [smallint] NOT NULL ,
	[MessageParameters] [varchar] (150) NULL ,
	[Workstation] [varchar] (63) NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sysroAuditActions] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroAuditActions] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroAuditElements] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroAuditElements] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[sysroAuditScreens] WITH NOCHECK ADD 
	CONSTRAINT [PK_sysroAuditScreens] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Audit] WITH NOCHECK ADD 
	CONSTRAINT [PK_Audit] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Audit] ADD 
	CONSTRAINT [DF_Audit_Date] DEFAULT (getdate()) FOR [Date]
GO


--INSERTAMOS DATOS ESTÁTICOS
-- SCREENS

INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(1	,'Browser',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(2	,'Employees',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(3	,'Scheduler',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(4	,'Shifts',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(5	,'Reports',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(6	,'CausesConcepts',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(7	,'Options',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(8	,'Terminals',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(9	,'Users',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(10	,'Export',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(11	,'JobStatus',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(12	,'JobMachines',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(13	,'JobIncidences',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(14	,'JobReports',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(15	,'JobOrderTemplate',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(16	,'AccStatus',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(17	,'AccGroups',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(18	,'AccTimeZone',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(19	,'AccZone',NULL)
INSERT INTO [dbo].[sysroAuditScreens]([ID], [Name], [Description]) VALUES(20	,'AccReport',NULL)

-- ACTIONS
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(1,'Connect',	NULL)
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(2,'Disconnect',	NULL)
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(3,'Insert',	NULL)
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(4,'Update',	NULL)
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(5,'Select',	NULL)
INSERT INTO [dbo].[sysroAuditActions]([ID], [Name], [Description]) VALUES(6,'Delete',	NULL)


-- ELEMENTS
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(1,'Employee'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(2,'EmployeeGroup'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(3,'Shift'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(4,'User',	'Obligatorio en operaciones de CONNECT y DISCONNECT')
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(5,'Card', 'Cambiamos identificador de tarjeta')
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(6,'FingerPrintBlock'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(7,'Contract',NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(8,'PunchMethod'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(9,'Mobility'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(10,'EmployeeData'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(11,'ProlongedAbsence'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(12,'MassiveCopy'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(13,'ShiftFromDate'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(14,'ShiftFromScratch'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(15,'Cause'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(16,'Accrual'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(17,'AccrualFromDate'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(18,'AccrualFromScratch'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(19,'AccrualGroup'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(20,'AccrualRule'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(21,'VisualTimeOptions'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(22,'Schedule'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(23,'HolidayTemplate'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(24,'Punch'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(25,'PunchCause'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(26,'Concept'	,NULL)
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(27,'ConceptGroup'	,NULL)

-- Añadimos columna UserConf a la tabla sysroUsers
ALTER TABLE [dbo].[sysroUsers] ADD [UserConfData] [Text] NULL
GO

-- Añadimos columna AllowVTAccess a la tabla sysroUserGroups
ALTER TABLE [dbo].[sysrousergroups] ADD [AllowVTAccess] [Bit] NULL
GO

-- Añadimos columna StartupValue para guardar el valor inicial anual para un acumulado y empleado
ALTER TABLE [dbo].[dailyAccruals] ADD [StartupValue] [Bit] NOT NULL DEFAULT (0)
GO

-- Añadimos columna LastUpdatedYear a las tabla EmployeeConceptAnnualLimits para guardar el último año al que se aplicó el valor inicial
ALTER TABLE [dbo].[EmployeeConceptAnnualLimits] ADD [LastUpdatedYear] [smallint] NOT NULL DEFAULT (0)
GO

-- Añadimos el bóton de informes para Producción
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Access\Reports','Reports','roFormReports.vbd','reports.ico','3111111111',1000,'NWR','')
GO

-- Añadimos el botón de Auditoría dentro de la sección de configuración
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Config\Audit','Audit','roFormAudit.vbd','Audit.ico','3111111111',1000,'NWR','')
GO

-- Añadimos el botón de Exportación Acumulados Andorra
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\Config\ExportAndorra','ExpAndorra','roFormExportAndorra.vbd','ExportAndorra.ico','3111111111',950,'NWR','Feature\AccrualsExportsAndorra')
GO

-- Modificaciones de modelo de datos para Exportaciones Andorra
alter table Concepts add IsExported bit DEFAULT(0)
GO
alter table Concepts add IsIntervaled bit DEFAULT(0)
GO
UPDATE CONCEPTS SET IsIntervaled=0
go
UPDATE CONCEPTS SET IsExported=0
go
CREATE TABLE [ConceptsPeriods] (
	[IDConcept] [smallint] NOT NULL ,
	[IDPeriod] [smallint] NOT NULL ,
	[BeginTime] [numeric](18, 6) NOT NULL ,
	[EndTime] [numeric](18, 6) NOT NULL ,
	[Per] [numeric](18, 6) NULL ,
	CONSTRAINT [PK_ConceptsPeriods] PRIMARY KEY  NONCLUSTERED 
	(
		[IDConcept],
		[IDPeriod]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

-- Creamos el Stored Procedure
CREATE PROCEDURE [dbo].[ActualizaGruposAcumulados] AS
BEGIN
	DECLARE @idConcept smallint;
	DECLARE @idGroup int;
	DECLARE @category nvarchar(128);
	DECLARE @categoryTmp nvarchar(128);
	DECLARE @totalCoincidences INT;
	DECLARE @posComa INT;
	DECLARE @conta INT;
	--
	DECLARE crs CURSOR FOR
		SELECT
			ID, Category
		FROM
			Concepts
		WHERE
			Category IS NOT NULL;   -- Hay que asegurar también que no es una cadena vacía (TODO)
	--

	OPEN crs;
	FETCH NEXT FROM crs INTO @idConcept, @category;
	SET @category = LTRIM(RTRIM(@category));
	WHILE @@FETCH_STATUS = 0 BEGIN
				--Procesamos el campo categoria
				SET @posComa = 1;
				WHILE @posComa != 0 BEGIN
				             SET @posComa = charindex(',',@category);
					IF @posComa= 0
						BEGIN
							SET @categoryTmp = @category;
						END
					ELSE
						BEGIN
                    SET @categoryTmp = LTRIM(RTRIM(SUBSTRING(@category, 1, @posComa-1)));
					          SET @category = RIGHT(@category, LEN(@category)-@posComa);
					   				-- @categoryTmp es el candidato a nuevo grupo para informes
						END			

					SELECT @idgroup=ID  FROM sysroReportGroups WHERE LTRIM(RTRIM(name)) = LTRIM(RTRIM(@categoryTmp));
					SET @totalCoincidences = (SELECT COUNT(*)  FROM sysroReportGroups WHERE LTRIM(RTRIM(name)) = LTRIM(RTRIM(@categoryTmp)));

					IF @totalCoincidences = 0 
						BEGIN
							INSERT INTO sysroReportGroups values (@categoryTmp);
							INSERT INTO sysroReportGroupConcepts (IDconcept, IDReportGroup) values (@idConcept,@@IDENTITY);
						END
					ELSE
						BEGIN
							-- Entonces se encontró un registro
							INSERT INTO sysroReportGroupConcepts (IDconcept, IDReportGroup) values (@idConcept,@idgroup);
						END		
          END;

			FETCH NEXT FROM crs INTO @idConcept, @category
	END;
	CLOSE crs;
	DEALLOCATE crs;
END
GO

-- Lo ejecutamos
EXEC ActualizaGruposAcumulados
GO

-- Lo eliminamos
DROP PROCEDURE [dbo].[ActualizaGruposAcumulados]
GO

-- Añadidos tabla de Reglas de acumulados
CREATE TABLE [AccrualsRules] (
  [IdAccrualsRule] [smallint] NULL ,
  [Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [Description] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [Definition] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [Schedule] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
  [BeginDate] [datetime] NULL ,
  [EndDate] [datetime] NULL ,
  [Priority] [smallint] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.DailyCauses DROP CONSTRAINT [PK_DailyCauses]
GO

ALTER TABLE [dbo].[DailyCauses] ADD [AccrualsRules] tinyint NOT NULL DEFAULT (0)
GO

ALTER TABLE [dbo].[DailyCauses] ADD 
	CONSTRAINT [PK_DailyCauses] PRIMARY KEY  NONCLUSTERED 
	(
	  [IDEmployee],
	  [Date],
	  [IDRelatedIncidence],
	  [IDCause],
	  [AccrualsRules]
	)  WITH FILLFACTOR = 90 ON [PRIMARY] 
GO



CREATE TABLE [EmployeeAccrualsRules] (
  [IDEmployee] [int] NOT NULL ,
  [IDAccrualsRules] [tinyint] NOT NULL ,
CONSTRAINT [PK_EmployeeAccrualsRules] PRIMARY KEY CLUSTERED 
(
  [IDEmployee],
  [IDAccrualsRules]
) ON [PRIMARY] 
) ON [PRIMARY]
GO

-- Borramos de la sysroTasks los registros referentes a CARRYOVERS
delete sysrotasks where ProcessID = 'PROC:\\CARRYOVERS'

-- Añadimos columnas nuevas para el redondeo de acumulados
ALTER TABLE [dbo].[Concepts] ADD [RoundConceptBy]  [numeric](18, 3) NULL 
GO

ALTER TABLE [dbo].[Concepts] ADD [RoundConceptType] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

-- Redondeo de los acumulados
update concepts set RoundConceptBy=1, RoundConceptType='~' where RoundConceptBy is null and RoundConceptType is null
GO

-- Añadimos columna RuleType a la tabla sysroShiftsCausesRules
ALTER TABLE [dbo].[sysroShiftsCausesRules] ADD [RuleType] [tinyint] NULL
GO

-- Las reglas existentes antes del SP4 corresponden todas a las simples. Por tanto actualizamos su código a 2 que será el correcto a partir del SP4
UPDATE sysroShiftsCausesRules SET RuleType=2 where RuleType is NULL
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='182' WHERE ID='DBVersion'
GO
