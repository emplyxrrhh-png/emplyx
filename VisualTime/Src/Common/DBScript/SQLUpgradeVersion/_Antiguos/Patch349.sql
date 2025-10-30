ALTER TABLE dbo.Employees ALTER COLUMN Name nvarchar(500) NULL
GO

ALTER TABLE dbo.sysroPassports ALTER COLUMN Name nvarchar(500) NULL
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,71,'TADIN',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,72,'TADIN',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,73,'TAEIPDIN',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,74,'TAEIPDIN',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

UPDATE sysroParameters SET Data='349' WHERE ID='DBVersion'
GO


