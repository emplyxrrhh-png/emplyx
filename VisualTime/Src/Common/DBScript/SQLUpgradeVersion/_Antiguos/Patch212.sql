--Se añade un nuevo campo en la tabla de acumulados que guarda cual es el acumulado de horas trabajadas en los informes de absentismo
ALTER TABLE Concepts ADD IsAccrualWork bit NULL
GO
UPDATE Concepts SET IsAccrualWork = 0
GO

-- Se añaden nuevos campos de produccion
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('i',1,'Campo9',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('j',1,'Campo10',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('k',1,'Campo11',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('l',1,'Campo12',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('m',1,'Campo13',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('n',1,'Campo14',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('o',1,'Campo15',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('p',1,'Campo16',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('q',1,'Campo17',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('r',1,'Campo18',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('s',1,'Campo19',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('t',1,'Campo20',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('u',1,'Campo21',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('v',1,'Campo22',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('w',1,'Campo23',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('x',1,'Campo24',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('y',1,'Campo25',0,'',0)
GO
INSERT INTO sysroProductionFields ([ID],[Order_Job] ,[Name] ,[TypeField],[DefaultValue] ,[AutoLauncher])  VALUES ('z',1,'Campo26',0,'',0)
GO

ALTER TABLE [dbo].[OrderTemplates] ADD 
	[Customi] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customj] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customk] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customl] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customm] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customn] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customo] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customp] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customq] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customr] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customs] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customt] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customu] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customv] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customw] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customx] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customy] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customz] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[Orders] ADD 
	[Customi] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customj] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customk] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customl] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customm] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customn] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customo] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customp] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customq] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customr] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customs] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customt] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customu] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customv] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customw] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customx] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customy] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Customz] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO


-- Aplicar justificaciones de ausencias prolongadas en días festivos
ALTER TABLE dbo.CAUSES ADD ApplyAbsenceOnHolidays bit null
GO
UPDATE dbo.CAUSES SET ApplyAbsenceOnHolidays=1 WHERE ApplyAbsenceOnHolidays IS NULL
GO


-- Se añaden los campos para el control de calidad
CREATE TABLE [dbo].[sysroJobQualityFields](
	[FieldName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
 CONSTRAINT [PK_sysroJobQualityFields] PRIMARY KEY NONCLUSTERED 
(
	[FieldName] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE JOBS ADD IsQualityJob bit null default(0)
GO
UPDATE JOBS SET IsQualityJob=0 where IsQualityJob is null
GO

ALTER TABLE JobTemplates ADD IsQualityJob bit null default(0)
GO
UPDATE JobTemplates SET IsQualityJob=0 where IsQualityJob is null
GO



-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='212' WHERE ID='DBVersion'
GO
