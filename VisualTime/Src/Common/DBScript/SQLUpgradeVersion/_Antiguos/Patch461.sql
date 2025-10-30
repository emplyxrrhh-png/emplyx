
insert into dbo.sysroGUI (IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
values('Portal\General\DataLinkBusiness','DataLink','DataLink/DataLinkBusiness.aspx','DataLink.png',NULL,'Passport:DatalinkMode:simple','Feature\DatalinkBusiness',NULL,103,NULL,'U:Employees.DataLink=Read OR U:Calendar.DataLink=Read OR U:Tasks.DataLink=Read OR U:BusinessCenters.DataLink=Read')
GO

update dbo.sysroGUI set Parameters ='Passport:DatalinkMode:advanced' where IDPath = 'Portal\General\DataLink'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.AdvancedDatalink')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.AdvancedDatalink','Advanced')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroQueries] WHERE Name = 'Permisos efectivos sobre funcionalidades concretas')
	insert into dbo.sysroQueries(Name,Description,Value) values ('Permisos efectivos sobre funcionalidades concretas','Muestra los permisos sobre las funcionalidades de Empleados y Calendario de los supervisores en V2 sobre empleados',
	'select tmp.SupervisorName as ''Supervisor'', tmp.EmployeeName as ''Empleado'', CASE when tmp.EmployeePermission = 9 Then ''Administración'' when tmp.EmployeePermission = 6 Then ''Escritura'' when tmp.EmployeePermission = 3 then ''Lectura'' when tmp.EmployeePermission = 0 then ''Sin permiso'' else ''Sin permiso'' end as ''F:Empleados'',
	CASE when tmp.CalendarPermission = 9 Then ''Administración'' when tmp.CalendarPermission = 6 Then ''Escritura'' when tmp.CalendarPermission = 3 then ''Lectura'' when tmp.CalendarPermission = 0 then ''Sin permiso'' else ''Sin permiso'' end as ''F:Calendario''	
	  from 
	(select srp.Name as SupervisorName,  emp.Name as EmployeeName, 
             dbo.WebLogin_GetPermissionOverEmployee(srp.ID, emp.ID, 1,0,1, GETDATE()) as EmployeePermission,
             dbo.WebLogin_GetPermissionOverEmployee(srp.ID, emp.ID, 2,0,1, GETDATE()) as CalendarPermission  
from sysroPassports srp, Employees emp    
where srp.IsSupervisor = 1 and  charindex(''@@ROBOTICS@@'',srp.Description) = 0 )  as tmp
where EmployeePermission > 0 or CalendarPermission > 0 order by Supervisor, Empleado')
GO


update dbo.sysroGUI_Actions set Section = 0 where Section =1
GO

-- EXPORTACIONES 
ALTER TABLE dbo.ExportGuides ADD
	Version smallint NULL,
	Concept nvarchar(20) NULL, 
	Active bit NULL
GO

update dbo.ExportGuides set Version = 0
GO
update dbo.ExportGuides set Version = 1 where len(ProfileMask)> 0
GO

INSERT INTO dbo.ExportGuides VALUES
 (20001,N'Exportación avanzada de empleados',N'',6,1,N'',N'',N'',0,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'1,1,0@1@1',N'',NULL,N'Employees.DataLink.Exports.AdvEmployees',N'Employees',NULL,NULL,NULL,2,N'Employees',0),
 (20002,N'Exportación avanzada de saldos',N'',1,1,N'',N'2',N'',1,N'',1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'1,1,0@1@1',N'',N'',N'Employees.DataLink.Exports.AdvAccruals',N'Employees',NULL,NULL,NULL,2,N'Accruals',0),
 (20003,N'Exportación avanzada de planificación',N'',12,1,N'',N'',N'',1,N'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'0,1,0@0@0',NULL,NULL,N'Calendar.DataLink.Exports.Calendar',N'Calendar',NULL,NULL,NULL,2,N'Schedule',0),
 (20004,N'Fichajes con plantilla',N'',8,1,N'',N'0',NULL,1,NULL,NULL,NULL,'',NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,N'1,1,0@1@1',NULL,NULL,N'Employees.DataLink.Exports.Punches',N'Employees',NULL,NULL,NULL,2,N'Punches',0);
 GO
 
-- Plantillas
CREATE TABLE [dbo].[ExportGuidesTemplates](
	[ID] [smallint] NOT NULL,
	[IDParentGuide] [smallint] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Profile] [nvarchar](128) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[PostProcessScript] [nvarchar](50) NULL,
	[PreProcessScript] [nvarchar](50) NULL,
 CONSTRAINT [PK_ExportGuidesTemplates] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ExportGuidesTemplates]  WITH CHECK ADD  CONSTRAINT [FK_ExportGuidesTemplates_ExportGuides] FOREIGN KEY([IDParentGuide])
REFERENCES [dbo].[ExportGuides] ([ID])
GO

ALTER TABLE [dbo].[ExportGuidesTemplates] CHECK CONSTRAINT [FK_ExportGuidesTemplates_ExportGuides]
GO

 INSERT INTO [dbo].[ExportGuidesTemplates] VALUES
 (1,20001,N'Básica',N'adv2_Emp',NULL,NULL,NULL),
 (2,20002,N'Básica',N'adv2_Accruals',NULL,NULL,NULL),
 (3,20003,N'Básica',N'CalendarLinkCellLayout',NULL,NULL,NULL),
 (4,20004,N'Básica',N'adv2_PunchesSDK',NULL,NULL,NULL);
 GO

--IMPORTACIONES 
ALTER TABLE dbo.ImportGuides ADD
	Version smallint NULL,
	Concept nvarchar(20) NULL,
	Active bit NULL
GO

update dbo.ImportGuides set Version = 0
GO

INSERT INTO dbo.ImportGuides VALUES
 (20,N'Empleados',0,1,1,'','',N'',1,'',N'Employees.DataLink.Imports.Employees',N'Employees',0,NULL,NULL,2,N'Employees',0),
 (21,N'Planificación',0,1,1,'','',N'',1,'',N'Calendar.DataLink.Imports.Schedule',N'Calendar',0,NULL,NULL,2,N'Schedule',0);

update dbo.ImportGuides set Version = 1 where ID = 14
GO

update dbo.ImportGuides set type = 1 where id = 14
GO

CREATE TABLE [dbo].[ImportGuidesTemplates](
	[ID] [smallint] NOT NULL,
	[IDParentGuide] [smallint] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Profile] [nvarchar](128) NOT NULL,
	[Parameters] [nvarchar](max) NULL,
	[PostProcessScript] [nvarchar](50) NULL,
	[PreProcessScript] [nvarchar](50) NULL,
 CONSTRAINT [PK_ImportGuidesTemplates] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ImportGuidesTemplates]  WITH CHECK ADD  CONSTRAINT [FK_ImportGuidesTemplates_ImportGuides] FOREIGN KEY([IDParentGuide])
REFERENCES [dbo].[ImportGuides] ([ID])
GO

ALTER TABLE [dbo].[ImportGuidesTemplates] CHECK CONSTRAINT [FK_ImportGuidesTemplates_ImportGuides]
GO


INSERT INTO [dbo].[ImportGuidesTemplates] VALUES
 (1,20,N'Básica',N'',NULL,NULL,NULL),
 (2,21,N'Básica',N'CalendarLinkCellLayout',NULL,NULL,NULL);
 GO

 ALTER TABLE dbo.ExportGuides ADD IDDefaultTemplate smallint NULL
GO

UPDATE eg
SET eg.IDDefaultTemplate = egt.ID
FROM dbo.ExportGuides  eg
INNER JOIN dbo.ExportGuidesTemplates egt on eg.id = egt.idparentguide AND eg.Version= 2
GO

ALTER TABLE dbo.ImportGuides ADD IDDefaultTemplate smallint NULL
GO

UPDATE ig
SET ig.IDDefaultTemplate = igt.ID
FROM dbo.ImportGuides  ig
INNER JOIN dbo.ImportGuidesTemplates igt on ig.id = igt.idparentguide AND ig.Version= 2
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTLive.ServerName')
insert into sysroLiveAdvancedParameters values ('VTLive.ServerName', '')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTPortal.MailRequest')
insert into sysroLiveAdvancedParameters values ('VTPortal.MailRequest', 'false')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='461' WHERE ID='DBVersion'
GO
