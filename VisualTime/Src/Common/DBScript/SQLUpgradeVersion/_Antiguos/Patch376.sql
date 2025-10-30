INSERT INTO dbo.sysroGUI VALUES ('LivePortal\CostControl','LivePortal.CostControl','','CostControl.png',NULL,NULL,NULL,NULL,1410,NULL,NULL)
GO

INSERT INTO dbo.sysroGUI VALUES ('LivePortal\CostControl\CCMoves','LivePortal.CCMoves','CostControl/CCMoves.aspx','Punches.png',NULL,NULL,'Feature\CostControl',NULL,1510,NULL,'E:Punches.Query=Read')
GO

UPDATE [dbo].[sysroFeatures] SET Alias = 'Employees.DataLink.Exports.StdEmployees' WHERE Alias = 'Employee.DataLink.Exports.StdEmployees'
GO

UPDATE [dbo].[sysroFeatures] SET Alias = 'Employees.DataLink.Exports.Sage' WHERE Alias = 'Employee.DataLink.Exports.Sage'
GO

CREATE FUNCTION [dbo].[GetEmployeeBiometricsIDLive]
  (				
  	
  )
  RETURNS @ValueTable table(idPassport integer, RXA100_0 datetime,RXA100_1 datetime, RXFFNG_0 datetime, RXFFNG_1 datetime, RXFFAC_0 datetime)
  
  AS
  BEGIN
	declare @idPassport integer
	declare @idPassportAnt integer
	declare @Version as nvarchar(max)
	declare @BiometricID as integer 
	declare @LenBiometricData integer 
	declare @TimeStamp as datetime
	declare @RXA100_0 datetime
	declare @RXA100_1 datetime
	declare @RXFFNG_0 datetime
	declare @RXFFNG_1 datetime
	declare @RXFFAC_0 datetime

	SET @RXA100_0 = null
	SET @RXA100_1 = null
	SET @RXFFNG_0 = null
	SET @RXFFNG_1 = null
	SET @RXFFAC_0  = null
	
	set @idPassportAnt =0	

	declare TableCursor cursor fast_forward for
		select PAM.IDPassport, PAM.Version, PAM.BiometricID, TimeStamp, (len(dbo.f_BinaryToBase64(PAM.biometricdata))) as LenBiometricData
		from sysroPassports_AuthenticationMethods PAM				
		where PAM.Method =4 and PAM.Enabled=1 And PAM.Version IN ('RXA100','RXFFNG','RXFFAC')
		order by PAM.IDPassport 

	open TableCursor
  	
  	fetch next from TableCursor into @idPassport, @Version, @BiometricID,@TimeStamp, @LenBiometricData		

	while (@@FETCH_STATUS <> -1)
	begin
		if @idPassportAnt = 0 set @idPassportAnt=@idPassport
				
		if @idPassportAnt<>@idPassport
			begin
				insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)
				set @RXA100_0= null
				set @RXA100_1= null
				set @RXFFNG_0= null
				set @RXFFNG_1= null
				set @RXFFAC_0= null

				set @idPassportAnt = @idPassport
			end

		if @Version='RXA100' and @BiometricID=0 set @RXA100_0=@TimeStamp
		if @Version='RXA100' and @BiometricID=1 set @RXA100_1=@TimeStamp
		if @Version='RXFFNG' and @BiometricID=0 AND @LenBiometricData>4 set @RXFFNG_0=@TimeStamp
		if @Version='RXFFNG' and @BiometricID=1 AND @LenBiometricData>4 set @RXFFNG_1=@TimeStamp
		if @Version='RXFFAC' and @BiometricID=0 AND @LenBiometricData>4 set @RXFFAC_0=@TimeStamp
							
		fetch next from TableCursor into @idPassport, @Version, @BiometricID, @TimeStamp, @LenBiometricData
	end

	if @idPassportAnt<>0  Insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0)

	close TableCursor
	deallocate TableCursor
  	RETURN
  END
GO

ALTER TABLE [dbo].[Terminals] ADD [SerialNumber] [nvarchar](32) NULL
GO
ALTER TABLE [dbo].[Terminals] ADD [RegistrationCode] [nvarchar](32) NULL
GO

ALTER TABLE [dbo].[sysroReportTasks] ADD 
	[ExecutionDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Progress] [int] NULL
GO

insert into dbo.sysroLiveAdvancedParameters(ParameterName,Value) values('MaxNumberOfReportThreads','1')
GO

insert into dbo.sysroLiveAdvancedParameters(ParameterName,Value) values('ReportsPersistOnSystem','3')
GO

INSERT INTO dbo.sysroGUI([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
	VALUES('Portal\General\TasksQueue','TasksQueue','Alerts/TasksQueue.aspx','TasksQueue.png',NULL,NULL,NULL,NULL,105,NULL,'U:Administration.TasksQueue=Read')

INSERT INTO dbo.sysroFeatures([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID]) 
	VALUES(7640,7,'Administration.TasksQueue','Lista de tareas','','U','RWA',NULL,NULL,NULL)


INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 7640, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 7
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

-- Nuevos comportamientos para mx8
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,90,'TAEIPCO',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,91,'TATSKCO',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

-- Centralita de accesos mxC
ALTER TABLE [dbo].[Terminals] ADD [SerialNumber] [nvarchar](32) NULL
GO
ALTER TABLE [dbo].[Terminals] ADD [RegistrationCode] [nvarchar](32) NULL
GO

ALTER TABLE [dbo].[TerminalsSyncTasks] ADD [Parameter1] integer NULL
GO
ALTER TABLE [dbo].[TerminalsSyncTasks] ADD [Parameter2] integer NULL
GO
ALTER TABLE [dbo].[TerminalsSyncTasks] ADD [TaskData] nvarchar(max) NULL
GO
ALTER TABLE [dbo].[TerminalsSyncTasks] ADD [TaskSent] [datetime] NULL
GO
ALTER TABLE [dbo].[TerminalsSyncTasks] ADD [TaskRetries] [smallint] NULL
GO

ALTER TABLE [dbo].[TerminalsSyncTasks]
ALTER COLUMN Task nvarchar(30) NOT NULL
GO

-- Creamos comportamientos para terminal rxInbio
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',1,80,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',1,81,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',2,82,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',2,83,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO dbo.sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxC',2,84,'',NULL,NULL,NULL,NULL,'0',NULL,NULL,NULL,NULL)
GO
-- Añadimos el campo IDReportTask a a todas las tablas temporales
DELETE FROM dbo.TMPCALENDAREMPLOYEE
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEE DROP CONSTRAINT [PK_TMPCALENDAREMPLOYEE]
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEE ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEE ADD CONSTRAINT [PK_TMPCALENDAREMPLOYEE] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC,
	[MES] ASC
)
GO

DELETE FROM dbo.TMPMONTHLYEMPLOYEECALENDAR
GO
ALTER TABLE dbo.TMPMONTHLYEMPLOYEECALENDAR ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPSHIFTDEFINITIONS
GO
ALTER TABLE dbo.TMPSHIFTDEFINITIONS ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPTOP
GO
ALTER TABLE dbo.TMPTOP ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPEmergency
GO
ALTER TABLE dbo.TMPEmergency DROP CONSTRAINT [PK_TMPEmergency]
GO
ALTER TABLE dbo.TMPEmergency ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPEmergency ADD CONSTRAINT [PK_TMPEmergency] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC,
	[UserFieldValue] ASC
)
GO


DELETE FROM dbo.TMPDailyPartialAccruals
GO
ALTER TABLE dbo.TMPDailyPartialAccruals ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPCONTROLHOLIDAYS
GO
ALTER TABLE dbo.TMPCONTROLHOLIDAYS ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO


DELETE FROM dbo.TMPUSERFIELDS
GO
ALTER TABLE dbo.TMPUSERFIELDS DROP CONSTRAINT [PK_TMPUSERFIELDS]
GO
ALTER TABLE dbo.TMPUSERFIELDS ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPUSERFIELDS ADD CONSTRAINT [PK_TMPUSERFIELDS] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC
)
GO

DELETE FROM dbo.TMPWhoShouldComeAndInst
GO
ALTER TABLE dbo.TMPWhoShouldComeAndInst ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TmpWeeklyScheduling
GO
ALTER TABLE dbo.TmpWeeklyScheduling DROP CONSTRAINT [PK_TmpWeeklyScheduling]
GO
ALTER TABLE dbo.TmpWeeklyScheduling ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TmpWeeklyScheduling ADD CONSTRAINT [PK_TmpWeeklyScheduling] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDGroup] ASC,
	[IDAssignment] ASC,
	[Type] ASC
	
)
GO

DELETE FROM dbo.TMPCALENDAREMPLOYEEByContract
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEEByContract DROP CONSTRAINT [PK_TMPCALENDAREMPLOYEEByContract]
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEEByContract ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEEByContract ADD CONSTRAINT [PK_TMPCALENDAREMPLOYEEByContract] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC,
	[idContract] ASC,
	[MES] ASC
	
)
GO

DELETE FROM dbo.TmpSchedulingLayerResume
GO
ALTER TABLE dbo.TmpSchedulingLayerResume ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TmpMonthlyScheduling
GO
ALTER TABLE dbo.TmpMonthlyScheduling DROP CONSTRAINT [PK_TmpMonthlyScheduling]
GO
ALTER TABLE dbo.TmpMonthlyScheduling ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TmpMonthlyScheduling ADD CONSTRAINT [PK_TmpMonthlyScheduling] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDGroup] ASC,
	[IDAssignment] ASC,
	[Type] ASC
	
)
GO


DELETE FROM dbo.TMPSchedulingCost
GO
ALTER TABLE dbo.TMPSchedulingCost DROP CONSTRAINT [PK_TMPSchedulingCost]
GO
ALTER TABLE dbo.TMPSchedulingCost ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPSchedulingCost ADD CONSTRAINT [PK_TMPSchedulingCost] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDGroup] ASC,
	[Date] ASC,
	[IDAssignment] ASC	
)
GO

DELETE FROM dbo.TMPEmergencyTotals
GO
ALTER TABLE dbo.TMPEmergencyTotals ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPEmergencyVisits
GO
ALTER TABLE dbo.TMPEmergencyVisits ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPHOLIDAYSCONTROL
GO
ALTER TABLE dbo.TMPHOLIDAYSCONTROL ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPHOLIDAYSCONTROLByContract
GO
ALTER TABLE dbo.TMPHOLIDAYSCONTROLByContract ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPANNUALINDIVIDUALCALENDAR
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDAR DROP CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDAR]
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDAR ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDAR ADD CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDAR] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC,
	[MES] ASC,
	[AÑO] ASC
)
GO

DELETE FROM dbo.TmpAnnualConceptsEmployee
GO
ALTER TABLE dbo.TmpAnnualConceptsEmployee DROP CONSTRAINT [PK_TmpAnnualConceptsEmployee]
GO
ALTER TABLE dbo.TmpAnnualConceptsEmployee ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TmpAnnualConceptsEmployee ADD CONSTRAINT [PK_TmpAnnualConceptsEmployee] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDPassport] ASC,
	[IDEmployee] ASC,
	[Ejercicio] ASC,
	[Periodo] ASC
)
GO

DELETE FROM dbo.TMPDetailedCalendarEmployee
GO
ALTER TABLE dbo.TMPDetailedCalendarEmployee DROP CONSTRAINT [PK_TMPDetailedCalendarEmployee]
GO
ALTER TABLE dbo.TMPDetailedCalendarEmployee ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPDetailedCalendarEmployee ADD CONSTRAINT [PK_TMPDetailedCalendarEmployee] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IdGroup] ASC,
	[IDEmployee] ASC,
	[DatePlanification] ASC
)
GO


DELETE FROM dbo.TMPANNUALINDIVIDUALCALENDARByContract
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDARByContract DROP CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDARByContract]
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDARByContract ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPANNUALINDIVIDUALCALENDARByContract ADD CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDARByContract] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDEmployee] ASC,
	[MES] ASC,
	[AÑO] ASC,
	[idContract] ASC

)
GO

DELETE FROM dbo.TMPCALENDAREMPLOYEEWeekly
GO
ALTER TABLE dbo.TMPCALENDAREMPLOYEEWeekly ADD IDReportTask Numeric(16,0) NULL DEFAULT(0)
GO

DELETE FROM dbo.TMPIndicatorsAnalysis
GO
ALTER TABLE dbo.TMPIndicatorsAnalysis DROP CONSTRAINT [PK_TMPIndicatorsAnalysis]
GO
ALTER TABLE dbo.TMPIndicatorsAnalysis ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPIndicatorsAnalysis ADD CONSTRAINT [PK_TMPIndicatorsAnalysis] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDGroup] ASC,
	[IDIndicator] ASC

)
GO

DELETE FROM dbo.TMPGroupIndicatorsAnalysis
GO
ALTER TABLE dbo.TMPGroupIndicatorsAnalysis DROP CONSTRAINT [PK_TMPGroupIndicatorsAnalysis]
GO
ALTER TABLE dbo.TMPGroupIndicatorsAnalysis ADD IDReportTask Numeric(16,0) NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.TMPGroupIndicatorsAnalysis ADD CONSTRAINT [PK_TMPGroupIndicatorsAnalysis] PRIMARY KEY NONCLUSTERED 
(
	[IDReportTask] ASC,
	[IDGroupParent] ASC,
	[IDGroup] ASC,
	[IDIndicator] ASC

)
GO


UPDATE dbo.sysroParameters SET Data='376' WHERE ID='DBVersion'
GO