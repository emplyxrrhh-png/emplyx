ALTER TABLE dbo.ExportGuides ADD
	DisplayParameters nvarchar(128) NULL,
	Scheduler nvarchar(128) NULL
GO

ALTER FUNCTION [dbo].[GetEmployeeGroupTree]
 	(
 		@idEmployee int,
 		@idEmployeeGroup int,
 		@date datetime
 	)
 RETURNS @ValueTable table (ID int)
 AS
 	BEGIN
 		/* Returns the list of groups and sub-groups employee belongs to */
 		/* Only one of idEmployee or idEmployeeGroup should be specified */
 		
 		DECLARE @Path nvarchar(100)
 		/* Find out in which group employee is directly */
 		IF NOT @idEmployee IS NULL
 			SELECT @idEmployeeGroup = dbo.GetEmployeeGroup(@idEmployee, @date)
 		
 		/* If employee is in a group, return groups tree */
 		IF NOT @idEmployeeGroup IS NULL
 		BEGIN
 			SELECT @Path = Path
 			FROM Groups
 			WHERE ID = @idEmployeeGroup
 			
 			INSERT INTO @ValueTable 
 				SELECT * 
 				FROM dbo.SplitInt(@Path, '\')
 		END
 		RETURN
 	END
GO

UPDATE dbo.ExportGuides set DisplayParameters = '0,1,0@0@0' WHERE ID < 9004
GO 

UPDATE dbo.ExportGuides set DisplayParameters ='1,1,0@1@1' WHERE ID IN(9004,9005,9006,9007,9008)
GO

INSERT INTO [dbo].[ExportGuides]([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ProfileName],[Destination],[ExportFileName],[ExportFileType],[Separator],[StartCalculDay],[StartExecutionHour],[LastLog],[NextExecution],[Field_1],[Field_2],[Field_3],[Field_4],[Field_5],[Field_6],[IntervalMinutes],[Scheduler],[DisplayParameters])
     VALUES
           (9009,'Exportación avanzada de empleados','adv_Emp',6,1,'','','','','',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,'','1,1,0@1@1')
GO


UPDATE dbo.sysroFeatures SET EmployeeFeatureID =1 WHERE Alias like 'Employees.BusinessCenters'
GO

-- PROGRAMADOR DE EVENTOS
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (13000 ,Null ,'Events' ,'Planificador de Eventos' ,'' ,'U' ,'RWA' ,NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (13100 ,13000 ,'Events.Definition' ,'Definicion de Eventos' ,'' ,'U' ,'RWA' ,NULL)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (13000, 13100) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\Access\AccessGroups'
GO

update dbo.sysrogui Set Priority = Priority + 1 where IDPath like 'Portal\Access\AccessPeriods'
GO


INSERT INTO dbo.sysroGUI (IDPath,LanguageReference,URL,IconURL,[Type], [Parameters], RequiredFeatures, SecurityFlags, Priority,AllowedSecurity,RequiredFunctionalities)
VALUES('Portal\Access\Events', 'Events', 'Access/Events.aspx' ,	'Events.png'	,NULL	,NULL	,'Feature\Events',NULL	,103	,NULL	,'U:Events.Definition=Read')
GO



CREATE TABLE [dbo].[EventsScheduler](
	[ID] [smallint] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ShortName] [nvarchar](6) NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[Description] [nvarchar](max) NULL,

 CONSTRAINT [PK_EventsScheduler] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventAccessAuthorization](
	[IDEvent] [smallint] NOT NULL,
	[IDAuthorization] [smallint] NOT NULL,
 CONSTRAINT [PK_EventAccessAuthorization] PRIMARY KEY CLUSTERED 
(
	[IDEvent] ASC,
	[IDAuthorization] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EventAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_EventAccessAuthorization_Authorization] FOREIGN KEY([IDAuthorization])
REFERENCES [dbo].[AccessGroups] ([ID])
GO

ALTER TABLE [dbo].[EventAccessAuthorization] CHECK CONSTRAINT [FK_EventAccessAuthorization_Authorization]
GO

ALTER TABLE [dbo].[EventAccessAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_EventAccessAuthorization_Events] FOREIGN KEY([IDEvent])
REFERENCES [dbo].[EventsScheduler] ([ID])
GO

ALTER TABLE [dbo].[EventAccessAuthorization] CHECK CONSTRAINT [FK_EventAccessAuthorization_Events]
GO


UPDATE sysroParameters SET Data='351' WHERE ID='DBVersion'
GO
