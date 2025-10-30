ALTER TABLE dbo.Entries ADD
	Reliability int NULL
GO
ALTER TABLE dbo.Entries ADD CONSTRAINT
	DF_Entries_Reliability DEFAULT 100 FOR Reliability
GO

CREATE TABLE dbo.Requests
	(
	ID int NOT NULL,
	IDEmployee int NOT NULL,
	RequestType tinyint NOT NULL,
	RequestDate datetime NOT NULL,
	Comments text NULL,
	Status tinyint NOT NULL,
	StatusLevel smallint NULL,
	Date1 datetime NULL,
	Date2 datetime NULL,
	FieldName nvarchar(50) NULL,
	FieldValue text NULL,
	IDCause smallint NULL,
	Hours numeric(19, 4) NULL,
	IDShift smallint NULL,
	IDEmployeeExchange int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	PK_Requests PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	FK_Requests_Employees FOREIGN KEY
	(
	IDEmployee
	) REFERENCES dbo.Employees
	(
	ID
	)
GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	FK_Requests_Causes FOREIGN KEY
	(
	IDCause
	) REFERENCES dbo.Causes
	(
	ID
	) 
	
GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	FK_Requests_Shifts FOREIGN KEY
	(
	IDShift
	) REFERENCES dbo.Shifts
	(
	ID
	) 
	
GO
ALTER TABLE dbo.Requests ADD CONSTRAINT
	FK_Requests_Employees1 FOREIGN KEY
	(
	IDEmployeeExchange
	) REFERENCES dbo.Employees
	(
	ID
	) 
	
GO

CREATE TABLE dbo.RequestsApprovals
	(
	IDRequest int NOT NULL,
	IDPassport int NOT NULL,
	DateTime datetime NOT NULL,
	Status tinyint NOT NULL,
	StatusLevel smallint NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.RequestsApprovals ADD CONSTRAINT
	PK_RequestsApprovals PRIMARY KEY CLUSTERED 
	(
	IDRequest,
	IDPassport,
	DateTime
	) ON [PRIMARY]

GO
ALTER TABLE dbo.RequestsApprovals ADD CONSTRAINT
	FK_RequestsApprovals_Requests FOREIGN KEY
	(
	IDRequest
	) REFERENCES dbo.Requests
	(
	ID
	)
	
GO
ALTER TABLE dbo.RequestsApprovals ADD CONSTRAINT
	FK_RequestsApprovals_sysroPassports FOREIGN KEY
	(
	IDPassport
	) REFERENCES dbo.sysroPassports
	(
	ID
	)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20,NULL,'Punches','Fichajes','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20000,20,'Punches.Query','Consulta','','E','R',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20100,20,'Punches.Requests','Solicitudes','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20110,20100,'Punches.Requests.Forgotten','Fichajes Olvidados','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20120,20100,'Punches.Requests.Justify','Justificar fichaje existente','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (20130,20100,'Punches.Requests.ExternalParts','Partes externos','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21,NULL,'Planification','Planificación','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21000,21,'Planification.Query','Consulta','','E','R',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21100,21,'Planification.Requests','Solicitudes','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21110,21100,'Planification.Requests.ShiftChange','Cambio de horario','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21120,21100,'Planification.Requests.Vacations','Vacaciones','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21130,21100,'Planification.Requests.ShiftExchange','Intercambio horario','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21140,21100,'Planification.Requests.PlannedAbsence','Ausencia prevista','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (22,NULL,'UserFields','Ficha empleado','','E','RW',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (22000,22,'UserFields.Query','Consulta','','E','R',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (22100,22,'UserFields.Requests','Solicitudes','','E','RW',NULL)
GO

INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (1560,1500,'Employees.UserFields.Requests','Solicitudes','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2310,2300,'Calendar.Punches.Punches','Edición de fichajes','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2320,2300,'Calendar.Punches.Punches.Requests','Solicitudes','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2321,2320,'Calendar.Punches.Punches.Requests.Forgotten','Fichajes olvidados','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2322,2320,'Calendar.Punches.Punches.Requests.Justify','Justificar fichaje','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2323,2320,'Calendar.Punches.Punches.Requests.ExternalParts','Partes externos','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2500,2,'Calendar.Requests','Solicitudes','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2510,2500,'Calendar.Requests.ShiftChange','Cambio horario','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2520,2500,'Calendar.Requests.Vacations','Vacaciones','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2530,2500,'Calendar.Requests.ShiftExchange','Intercambio horario','','U','RWA',NULL)
GO
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (2540,2500,'Calendar.Requests.PlannedAbsence','Ausencia prevista','','U','RWA',NULL)
GO

ALTER TABLE dbo.sysroUserFields ADD
	RequestPermissions smallint NULL,
	RequestCriteria text NULL
GO
ALTER TABLE dbo.sysroUserFields ADD CONSTRAINT
	DF_sysroUserFields_VisibilityPermissions DEFAULT (0) FOR RequestPermissions
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='242' WHERE ID='DBVersion'
GO
