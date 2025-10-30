CREATE NONCLUSTERED INDEX [TerminalBiometricTasks]
ON [dbo].[TerminalsSyncBiometricData] ([TerminalId])
INCLUDE ([EmployeeId],[FingerId],[FingerData],[TimeStamp],[Version])
GO

ALTER TABLE [dbo].[terminals] ADD [UserCount] NVARCHAR(6) NULL
GO
ALTER TABLE [dbo].[terminals] ADD [FaceCount] NVARCHAR(6) NULL
GO
ALTER TABLE [dbo].[terminals] ADD [PalmCount] NVARCHAR(6) NULL
GO
ALTER TABLE [dbo].[terminals] ADD [FingerCount] NVARCHAR(6) NULL
GO
ALTER TABLE [dbo].[terminals] ADD [Wifi] NVARCHAR(1) NULL
GO

-- Control de repeticiones de ciertas notificaciones (APV)
ALTER TABLE [dbo].[sysroNotificationTasks] ADD Repetition [SMALLINT] NULL
GO
ALTER TABLE [dbo].[sysroNotificationTasks] ADD NextRepetition [SMALLDATETIME] NULL
GO

-- Más de 256 sirenas en un terminal mx8
ALTER TABLE [dbo].[TerminalSirens] DROP CONSTRAINT [PK_TerminalSirens]
GO

ALTER TABLE [dbo].[TerminalSirens]
ALTER COLUMN [ID] SMALLINT NOT NULL	
GO

ALTER TABLE [dbo].[TerminalSirens] ADD CONSTRAINT [PK_TerminalSirens] PRIMARY KEY NONCLUSTERED  (
	[IDTerminal] ASC,
	[ID] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER VIEW [dbo].[sysrovwEmployeeStatus]
 AS
	SELECT svEmployees.IdEmployee, 
	tmpAttendance.ShiftDate AttShiftDate, 
	tmpAttendance.DateTime as AttDatetime, 
	CASE WHEN tmpAttendance.DateTime < dateadd(hour, -12, getdate()) THEN 'Out-Calculated' ELSE CASE tmpAttendance.ActualType WHEN 1 THEN 'In' ELSE 'Out' END END as AttStatus, 
	tmpTasks.DateTime as TskDatetime, 
    Tasks.Project + ' : ' +  Tasks.Name as TskName,  
	tmpcosts.DateTime as CostDatetime, 
	BusinessCenters.Name as CostCenterName ,
	BusinessCenters.ID as CostIDCenter 
	FROM sysrovwcurrentemployeegroups svEmployees
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberAtt', * FROM Punches WHERE ActualType = 1 or ActualType = 2) tmpAttendance ON svEmployees.IDEmployee = tmpAttendance.IdEmployee
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberTsk', * FROM Punches WHERE ActualType = 4) tmpTasks ON svEmployees.IDEmployee = tmpTasks.IdEmployee
	left join Tasks ON Tasks.Id = tmpTasks.TypeData
	left join (SELECT row_number() OVER (PARTITION BY IdEmployee ORDER BY idemployee ASC, DateTime DESC) AS 'RowNumberCost', * FROM Punches WHERE ActualType = 13) tmpCosts ON svEmployees.IDEmployee = tmpCosts.IdEmployee
	left join BusinessCenters ON tmpCosts.TypeData = BusinessCenters.ID
	WHERE CurrentEmployee = 1
	and (RowNumberAtt = 1 or RowNumberAtt is null)
	and (RowNumberTsk = 1 or RowNumberTsk is null)
	and (RowNumberCost = 1 or RowNumberCost is null)
	and (tmpAttendance.DateTime IS NOT NULL OR tmpTasks.DateTime IS NOT NULL OR tmpCosts.DateTime IS NOT NULL)
GO

CREATE PROCEDURE [dbo].[Analytics_CostCenters_ActualStatus]
       @idpassport int,  
    @employeeFilter nvarchar(max),  
       @userFieldsFilter nvarchar(max),  
    @businessCenterFilter nvarchar(max)  
       AS  
    DECLARE @employeeIDs Table(idEmployee int)  
    DECLARE @pinitialDate smalldatetime = getdate(),    
       @pendDate smalldatetime = getdate(),    
       @pidpassport int = @idpassport,    
       @pemployeeFilter nvarchar(max) = @employeeFilter,    
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter,
  	 @pbusinessCenterFilter nvarchar(max) = @businessCenterFilter
    insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
    DECLARE @businessCenterIDs Table(idBusinessCenter int)  
    insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
        SELECT     CONVERT(varchar(10), dbo.sysrovwEmployeeStatus.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                              + '-' + dbo.EmployeeContracts.IDContract  AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                              dbo.EmployeeContracts.IDContract, dbo.sysrovwEmployeeStatus.IDEmployee, dbo.sysrovwEmployeeStatus.CostDatetime,  
                               isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
            isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
            isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5,
                              dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                              dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,   
               dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
            dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
            dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
            dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
            dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
            dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),getdate()) As UserField1,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),getdate()) As UserField2,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),getdate()) As UserField3,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),getdate()) As UserField4,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),getdate()) As UserField5,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),getdate()) As UserField6,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),getdate()) As UserField7,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),getdate()) As UserField8,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),getdate()) As UserField9,  
            dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10  
        FROM         dbo.sysroEmployeeGroups with (nolock)   
                              INNER JOIN dbo.sysrovwEmployeeStatus with (nolock) ON  dbo.sysroEmployeeGroups.IDEmployee = dbo.sysrovwEmployeeStatus.IDEmployee AND GETDATE() between dbo.sysroEmployeeGroups.BeginDate AND dbo.
  sysroEmployeeGroups.EndDate   
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.EmployeeContracts.IDEmployee AND GETDATE() >= dbo.EmployeeContracts.BeginDate AND GETDATE() <= dbo.EmployeeContracts.EndDate   
            INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
            INNER JOIN dbo.Employees with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.Employees.ID    
            LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.sysrovwEmployeeStatus.CostIDCenter,0) = dbo.BusinessCenters.ID   
         where   dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysrovwEmployeeStatus.IDEmployee,2,0,0,getdate()) > 1 AND      
       isnull(dbo.sysrovwEmployeeStatus.CostIDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
       AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.Notification.EnabledSchedulerAnalyticExecuted')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.Notification.EnabledSchedulerAnalyticExecuted','0')
GO


IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 71)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(71,'Scheduler Analytic Executed',null, 1, 1,'','')
GO

--Borrado de tareas antiguas de Broadcaster (no se trataban al reiniciar el servicio)
DELETE [dbo].[sysroLiveTasks] WHERE action like 'BROAD%' 
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 4900)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (4900,71,'Analitica planificada ejecutada','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,1,0,NULL)
GO

ALTER TABLE dbo.ReportLayoutCategories ADD
	CategoryLevel int NULL
GO

ALTER TABLE dbo.ReportLayouts ADD
	RequieredFeature nvarchar(100) NULL
GO

--Informe de identificadores biométricos
 ALTER FUNCTION [dbo].[GetEmployeeBiometricsIDLive]
     (				
     	
     )
     RETURNS @ValueTable table(idPassport integer, RXA100_0 datetime,RXA100_1 datetime, RXFFNG_0 datetime, RXFFNG_1 datetime, RXFFAC_0 datetime, RXFPAL_0 datetime)
     AS
     BEGIN
   	declare @idPassport integer
   	declare @idPassportAnt integer=0
   	declare @Version as nvarchar(max)
   	declare @BiometricID as integer 
   	declare @LenBiometricData integer 
   	declare @TimeStamp as datetime
  	declare @fnumber integer
   	declare @RXA100_0 datetime = null
   	declare @RXA100_1 datetime = null
   	declare @RXFFNG_0 datetime = null
   	declare @RXFFNG_1 datetime = null
  	declare @RXFFNG_2 datetime = null
  	declare @RXFFNG_3 datetime = null
  	declare @RXFFNG_4 datetime = null
  	declare @RXFFNG_5 datetime = null
  	declare @RXFFNG_6 datetime = null
  	declare @RXFFNG_7 datetime = null
  	declare @RXFFNG_8 datetime = null
  	declare @RXFFNG_9 datetime = null
   	declare @RXFFAC_0 datetime = null
 	declare @RXFPAL_0 datetime = null
   	
   	declare TableCursor cursor fast_forward for
  		select Numero, IDPassport, Version, BiometricID, TimeStamp, LenBiometricData from 
  		(
  		select ROW_NUMBER() OVER (PARTITION BY IDPassport,Version ORDER BY IdPassport, BiometricID ASC) Numero,  PAM.IDPassport, PAM.Version, PAM.BiometricID, TimeStamp, (len(dbo.f_BinaryToBase64(PAM.biometricdata))) as LenBiometricData from 
  		sysroPassports_AuthenticationMethods PAM				
  		where PAM.Method =4 and PAM.Enabled=1 And PAM.Version IN ('RXA100','RXFFNG','RXFFAC','ZKUNIFAC','ZKUNIPAL') And len(dbo.f_BinaryToBase64(PAM.biometricdata))>4
  		) aux 
  		where (Version <> 'ZKUNIPAL' AND Numero <= 2) OR (Version = 'ZKUNIPAL' and Numero = 1)
   	open TableCursor
     	
     	fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID,@TimeStamp, @LenBiometricData		
   	while (@@FETCH_STATUS <> -1)
   	begin
   		if @idPassportAnt = 0 set @idPassportAnt=@idPassport
   				
   		if @idPassportAnt<>@idPassport
   			begin
   				insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0, @RXFPAL_0)
   				set @RXA100_0= null
   				set @RXA100_1= null
   				set @RXFFNG_0= null
   				set @RXFFNG_1= null
   				set @RXFFAC_0= null
 				set @RXFPAL_0= null
   				set @idPassportAnt = @idPassport
   			end
   		if @Version='RXA100' and @BiometricID=0 set @RXA100_0=@TimeStamp
   		if @Version='RXA100' and @BiometricID=1 set @RXA100_1=@TimeStamp
   		if @Version='RXFFNG' and @fnumber=1 set @RXFFNG_0=@TimeStamp
   		if @Version='RXFFNG' and @fnumber=2 set @RXFFNG_1=@TimeStamp
   		if (@Version='RXFFAC' or @Version='ZKUNIFAC') and @BiometricID=0 AND @LenBiometricData>4 set @RXFFAC_0=@TimeStamp
 		if @Version='ZKUNIPAL' and @BiometricID=0 set @RXFPAL_0=@TimeStamp
   							
   		fetch next from TableCursor into @fnumber, @idPassport, @Version, @BiometricID, @TimeStamp, @LenBiometricData
   	end
   	if @idPassportAnt<>0  Insert into @ValueTable values (@idPassportAnt, @RXA100_0, @RXA100_1, @RXFFNG_0, @RXFFNG_1, @RXFFAC_0, @RXFPAL_0)
   	close TableCursor
   	deallocate TableCursor
     	RETURN
     END
GO




UPDATE sysroParameters SET Data='506' WHERE ID='DBVersion'
GO

