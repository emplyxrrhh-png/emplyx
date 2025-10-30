-- Fecha de cierre por empleado
IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'LockDateWzd' AND IDGUIPath = 'Portal\General\Employees\Employees')
	INSERT into sysroGUI_Actions values ('LockDateWzd', 'Portal\General\Employees\Employees', 'tbLockDateWizard', 'Forms\Employees', 'U:Employees=Admin', 'ShowLockDateWizard()', 'btnTbLockDate', 0, 7, 1)
GO

alter table Employees ADD LockDate smalldatetime NULL
GO

CREATE TABLE [dbo].[sysroDateParameters](
	[ParameterName] [nvarchar](100) NOT NULL,
	[Value] [smalldatetime] NULL,
 CONSTRAINT [PK_sysroDateParameters] PRIMARY KEY CLUSTERED 
(
	[ParameterName] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

INSERT INTO sysroDateParameters (ParameterName, Value) Values('FirstDate', NULL)
GO

CREATE VIEW [dbo].[sysrovwEmployeeLockDate]
AS
 SELECT ID as IDEmployee, isnull(LockDate, (select isnull(Value,convert(smalldatetime, '1900/01/01', 120))  
 FROM sysroDateParameters WHERE ParameterName ='FirstDate')) as LockDate , case when ISNULL(lockdate,convert(smalldatetime, '1900/01/01', 120)) = convert(smalldatetime, '1900/01/01', 120)   then 0 else 1 end as EmployeeLockDateType
 FROM Employees  
GO


ALTER TABLE dbo.ExportGuides ADD ApplyLockDate bit NOT NULL DEFAULT(0)
GO

-- añadimos fecha de cierre del empleado en la analitica de planificacion
 ALTER PROCEDURE  [dbo].[Analytics_Schedule]
     	@initialDate smalldatetime,
     	@endDate smalldatetime,
     	@idpassport int,
     	@employeeFilter nvarchar(max),
     	@userFieldsFilter nvarchar(max)
      AS
      DECLARE @employeeIDs Table(idEmployee int)
  	 DECLARE @pinitialDate smalldatetime = @initialDate,  
       @pendDate smalldatetime = @endDate,  
       @pidpassport int = @idpassport,  
       @pemployeeFilter nvarchar(max) = @employeeFilter,  
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
      SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
  		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
  		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
  		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
  		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
  		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
  		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
  		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
  		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
  		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
  		dbo.sysroEmployeesShifts.IDAssignment,
  		CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark) as Remark,
  		dbo.sysroEmployeesShifts.AssignmentName,
  		dbo.sysroEmployeesShifts.IDProductiveUnit,
  		dbo.sysroEmployeesShifts.ProductiveUnitName, 
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField1,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField2,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField3,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField4,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField5,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField6,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField7,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField8,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField9,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField10,
		(SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate
      FROM         dbo.sysroEmployeesShifts with (nolock) 
  		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
  		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
  		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
  	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
  		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
  	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
  		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
  		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
  		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
  		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
  		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
  		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
  		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark)
GO


alter VIEW [dbo].[sysrovwAllEmployeeGroups]  
 AS  
 SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate  
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled  
 , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,   
     sysrovwCurrentEmployeeGroups.isTransfer,'ERROR' As CostCenter  
 FROM            dbo.sysrovwGetEmployeeGroup geg  
 INNER JOIN dbo.sysrovwCurrentEmployeeGroups  
 ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee and getdate() between geg.BeginDate and geg.EndDate  
 UNION  
 SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate  
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled  
 , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,   
     sysrovwFutureEmployeeGroups.isTransfer,'ERROR' As CostCenter  
 FROM            dbo.sysrovwGetEmployeeGroup geg  
 INNER JOIN dbo.sysrovwFutureEmployeeGroups   
  ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee and geg.BeginDate > getdate() 
  GO

ALTER VIEW [dbo].[sysrovwAllEmployeeGroupsWithCostCenter]
 AS
 SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwCurrentEmployeeGroups.isTransfer,dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwCurrentEmployeeGroups.IDEmployee,sysrovwCurrentEmployeeGroups.BeginDate) As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwCurrentEmployeeGroups
 ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee and getdate() between geg.BeginDate and geg.EndDate  
 UNION
 SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate
 , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled
 , geg.FullGroupName, geg.IDCompany, 
    sysrovwFutureEmployeeGroups.isTransfer,dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwFutureEmployeeGroups.IDEmployee,sysrovwFutureEmployeeGroups.BeginDate) As CostCenter
 FROM            dbo.sysrovwGetEmployeeGroup geg
 INNER JOIN dbo.sysrovwFutureEmployeeGroups 
 ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee and geg.BeginDate > getdate() 
GO

-- Nueva notificacion de fichajes en periodo de cierre
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroNotificationTypes] WHERE ID = 68)
	INSERT INTO dbo.sysroNotificationTypes(ID,Name,Description,Scheduler,OnlySystem, Feature,FeatureType) VALUES(68,'Punches in lock date  ',null, 360, 1,'Employees','U')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notifications] WHERE ID = 904)
	INSERT INTO [dbo].[Notifications] ([ID],[IDType],[Name],[Condition],[Destination],[Schedule],[Activated],[CreatorID],[AllowPortal],[IDPassportDestination],[AllowMail],[ShowOnDesktop],[LastTaskDeleted])
     VALUES (904,68,'Fichajes en periodo de cierre','<?xml version="1.0"?><roCollection version="2.0"></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>',NULL,1,'SYSTEM',0,0,0,1,NULL)
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='470' WHERE ID='DBVersion'
GO
