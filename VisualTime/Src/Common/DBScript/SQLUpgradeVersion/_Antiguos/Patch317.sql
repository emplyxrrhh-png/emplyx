DROP VIEW sysrovwCurrentEmployeesPresenceStatusPunches
GO

-- Crea la vista de nuevo para que determinadas salidas justificadas no se consideren SALIDA sino ENTRADA
-- Las justificaciones que cuando se fichen como salida deben ser consideradas como entrada son aquellas cuya descripción contiene el texto #EmergencyIn#
CREATE VIEW sysrovwCurrentEmployeesPresenceStatusPunches As
SELECT     p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, CASE WHEN ActualType IN (1, 2) 
                      THEN 'Att' ELSE 'Acc' END AS MoveType, ISNULL(p.IDZone, 0) AS IDZone, 
                      CASE WHEN ActualType = 1 THEN 'IN' WHEN ActualType = 2 THEN (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) ELSE CASE zo.IsWorkingZone WHEN 1 THEN 'IN' ELSE (CASE WHEN CHARINDEX('#EmergencyIn#', dbo.Causes.Description) 
                      > 0 THEN 'IN' ELSE 'OUT' END) END END AS Status
FROM         dbo.Punches AS p INNER JOIN
                          (SELECT     dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                            FROM          dbo.Punches INNER JOIN
                                                       (SELECT     IDEmployee, MAX(DateTime) AS dat
                                                         FROM          dbo.Punches AS Punches_1
                                                         WHERE      (ActualType IN (1, 2)) OR
                                                                                (Type = 5)
                                                         GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                            WHERE      (dbo.Punches.ActualType IN (1, 2)) OR
                                                   (dbo.Punches.Type = 5)
                            GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp INNER JOIN
                      dbo.Causes ON p.TypeData = dbo.Causes.ID LEFT OUTER JOIN
                      dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
                      dbo.Employees AS em ON p.IDEmployee = em.ID
					  
GO


--Pantalla administración
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES('Portal\Security\Aministration','GUI.SaaSAdministration','Security/SaaSAdmin.aspx','AdminSaaS.png',NULL,'RoboticsEmployee','Forms\Passports',NULL,300,NULL,'U:Administration.Security=Read')
GO

INSERT INTO [dbo].[sysroParameters]([ID],[Data])
     VALUES ('ACTIVE','1')
GO

CREATE TABLE dbo.sysroPassports_SaaS_Status
	(
	IDPassport int NOT NULL,
	EnabledVTDesktop bit NOT NULL,
	EnabledVTSupervisor bit NOT NULL,
	CONSTRAINT [PK_sysroPassports_SaaS_Status] PRIMARY KEY CLUSTERED 
	(
		IDPassport ASC
	) ON [PRIMARY]
	)  ON [PRIMARY]
GO

-- Informes en background
CREATE TABLE [dbo].[sysroReportTasks](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDPassport] [int] NOT NULL,
	[ReportName] [nvarchar](max) NOT NULL,
	[FileName] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[Parameters] [nvarchar](max) NOT NULL,
	[Culture] [nvarchar](max) NULL,
	[UploadFile] [nvarchar](max) NULL,
	[ExportFormatType] [int] NULL,
	[TimeStamp] [datetime] NULL,
 CONSTRAINT [PK_sysroReportTasks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[sysroReportTasks] ADD  CONSTRAINT [DF_sysroReportTasks_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[sysroReportTasks] ADD  CONSTRAINT [DF_sysroReportTasks_ExportFormatType]  DEFAULT ((5)) FOR [ExportFormatType]
GO

ALTER TABLE [dbo].[sysroReportTasks] ADD  CONSTRAINT [DF_sysroReportTasks_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

CREATE TABLE [dbo].[sysroMonitorValues](
	[Name] [nvarchar](50) NOT NULL,
	[TimeStamp] [smalldatetime] NOT NULL,
	[Value] [decimal](18, 3) NOT NULL,
 CONSTRAINT [PK_SysroMonitorValues] PRIMARY KEY CLUSTERED 
(
	[Name] ASC,
	[TimeStamp] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SysroMonitorValues] ADD  CONSTRAINT [DF_SysroMonitorValues_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

ALTER TABLE [dbo].[SysroMonitorValues] ADD  CONSTRAINT [DF_SysroMonitorValues_Value]  DEFAULT ((0)) FOR [Value]
GO

update [sysroReaderTemplates] set [OHP]='1,0' where ID in (41,42,43)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='317' WHERE ID='DBVersion'
GO

