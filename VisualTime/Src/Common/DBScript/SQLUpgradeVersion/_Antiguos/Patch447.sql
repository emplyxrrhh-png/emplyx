ALTER TABLE dbo.TMPTOP ALTER COLUMN ConceptPrimaryName NVARCHAR(100)
GO

ALTER TABLE dbo.TMPTOP ALTER COLUMN ConceptSecundaryName NVARCHAR(100)
GO

ALTER TABLE dbo.sysroPassports_Data ADD
	IsSupervisor bit NULL default(0)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =21130)
BEGIN
	INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees]) VALUES (21130,21100,'Planification.Requests.ShiftExchange','Intercambio horario','','E','RW',NULL)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2580)
BEGIN
	INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID]) VALUES (2580,2500,'Calendar.Requests.ShiftExchange','Intercambio horario','','U','RWA',NULL,8,2)

	INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2580, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2500	
END
GO

exec dbo.sysro_GenerateAllPermissionsOverFeatures
GO

-- Cálculo de valores iniciales de saldos
ALTER TABLE [dbo].[StartupValues] ADD
	[StartValueBaseType] smallint null default(0),
	[StartValueBase] [nvarchar](50) NULL,
	[TotalPeriodBaseType] smallint null default(0),
	[TotalPeriodBase] [nvarchar](50) NULL,
	[AccruedValueType] smallint null default(0),
	[AccruedValue] [nvarchar](50) NULL,
	[RoundingType] smallint NOT NULL DEFAULT(0)
 GO


update sysroSecurityNode_Passports_PermissionsOverEmployeesExceptions set IDSecurityNode=0
GO

insert into sysroGUI_Actions values('CommPwd','Portal\GeneralManagement\Terminal\List','tbChangePwdComms','Forms\Terminals','U:Terminals.StatusInfo=Read','ChangeTerminalPwd()','btnTbCommsPwd2',0,5)
GO

ALTER TABLE [dbo].[Terminals] ADD ForceConfig bit
GO

ALTER TABLE [dbo].[ProgrammedAbsences] ADD
	[IsExported] [bit] NULL DEFAULT (0)
GO

UPDATE ProgrammedAbsences SET IsExported =1 where IsExported is null 
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='447' WHERE ID='DBVersion'
GO