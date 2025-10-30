UPDATE [dbo].[sysroGui] SET Parameters='SaasEnabled' WHERE URL LIKE '%Surveys%'
GO

ALTER TABLE [dbo].[Punches] ADD Source INT DEFAULT 0
GO

UPDATE [dbo].[Punches] SET Source = 0
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuides] WHERE ID = 22)
INSERT [dbo].[ImportGuides] ([ID], [Name], [Template], [Mode], [Type], [FormatFilePath], [SourceFilePath], [Separator], [CopySource], [LastLog], [RequieredFunctionalities], [FeatureAliasID], [Destination], [Parameters], [Enabled], [Version], [Concept], [IDDefaultTemplate], [Active], [PreProcessScript], [Edition]) 
                     VALUES (22, N'Ausencias', 0, 1, 1, NULL, N'', N';', 1, N'', N'Calendar.DataLink.Imports.Schedule', N'Calendar', 2, NULL, 1, 2, N'Absences', 3, NULL, NULL, N'')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuides] WHERE ID = 23)
INSERT [dbo].[ImportGuides] ([ID], [Name], [Template], [Mode], [Type], [FormatFilePath], [SourceFilePath], [Separator], [CopySource], [LastLog], [RequieredFunctionalities], [FeatureAliasID], [Destination], [Parameters], [Enabled], [Version], [Concept], [IDDefaultTemplate], [Active], [PreProcessScript], [Edition]) 
                     VALUES (23, N'Justificaciones', 0, 1, 1, NULL, N'', N'', 1, N'', N'Calendar.DataLink.Imports.Schedule', N'Calendar', 2, NULL, 1, 2, N'Causes', 4, NULL, NULL, N'')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuidesTemplates] WHERE ID = 3)
INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript]) 
                              VALUES (3, 22, N'Básica', N'tmplImportAbsences.xlsx', NULL, NULL, NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ImportGuidesTemplates] WHERE ID = 4)
INSERT [dbo].[ImportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript]) 
                              VALUES (4, 23, N'Básica', N'tmplImportCauses.xlsx', NULL, NULL, NULL)
GO

alter table tmpdetailedCalendarEmployee alter column EmployeeName [varchar] (300) NULL
GO

alter table TMPDetailedCalendarEmployee alter column [GroupName] [nvarchar](500) NULL 
GO

CREATE TABLE [dbo].[GeniusCustomReportCheckboxes](
	[IdCheck] [int] NOT NULL,
	[CheckName] [nvarchar](20) NOT NULL,
	[Order] [int] NULL,
 CONSTRAINT [PK_GeniusCustomReportConfig] PRIMARY KEY CLUSTERED 
(
	[IdCheck] ASC
)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[GeniusCustomReportViews](
	[Class] [nvarchar](10) NOT NULL,
	[NameViewResult] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_GenisCustomReportViews] PRIMARY KEY CLUSTERED 
(
	[Class] ASC
)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[GeniusCustomReportCombs](
	[IdComb] [int] IDENTITY(1,1) NOT NULL,
	[IdCheck] [int] NOT NULL,
	[Class] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_GeniusCustomReportCombs] PRIMARY KEY CLUSTERED 
(
	[IdComb] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GeniusCustomReportCombs]  WITH CHECK ADD  CONSTRAINT [FK_GeniusCustomReportCombs_GenisCustomReportViews] FOREIGN KEY([Class])
REFERENCES [dbo].[GeniusCustomReportViews] ([Class])
GO

ALTER TABLE [dbo].[GeniusCustomReportCombs] CHECK CONSTRAINT [FK_GeniusCustomReportCombs_GenisCustomReportViews]
GO

ALTER TABLE [dbo].[GeniusCustomReportCombs]  WITH CHECK ADD  CONSTRAINT [FK_GeniusCustomReportCombs_GeniusCustomReportCheckboxes] FOREIGN KEY([IdCheck])
REFERENCES [dbo].[GeniusCustomReportCheckboxes] ([IdCheck])
GO

ALTER TABLE [dbo].[GeniusCustomReportCombs] CHECK CONSTRAINT [FK_GeniusCustomReportCombs_GeniusCustomReportCheckboxes]
GO

ALTER TABLE [dbo].[GeniusCustomReportViews]  WITH CHECK ADD  CONSTRAINT [FK_GenisCustomReportViews_GenisCustomReportViews] FOREIGN KEY([Class])
REFERENCES [dbo].[GeniusCustomReportViews] ([Class])
GO

ALTER TABLE [dbo].[GeniusCustomReportViews] CHECK CONSTRAINT [FK_GenisCustomReportViews_GenisCustomReportViews]
GO

ALTER TABLE [dbo].[GeniusCustomReportViews]  WITH CHECK ADD  CONSTRAINT [FK_GenisCustomReportViews_GenisCustomReportViews1] FOREIGN KEY([Class])
REFERENCES [dbo].[GeniusCustomReportViews] ([Class])
GO

ALTER TABLE [dbo].[GeniusCustomReportViews] CHECK CONSTRAINT [FK_GenisCustomReportViews_GenisCustomReportViews1]
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 1)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (1,'justifications',1)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 2)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (2,'fichajes',2)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 4)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (4,'costcenters',4)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 5)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (5,'requests',3)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 6)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (6,'saldes',6)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 7)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (7,'schedule',5)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 8)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (8,'tasks',8)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCheckboxes] WHERE IdCheck = 9)
INSERT INTO [dbo].[GeniusCustomReportCheckboxes]
           ([IdCheck],[CheckName],[Order])
     VALUES
           (9,'access',7)
GO

DELETE FROM [dbo].[GeniusCustomReportCombs]
      
GO

DELETE FROM [dbo].[GeniusCustomReportViews]
      
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'accesos')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('accesos'
           ,'authorizationsAccessByEmployee')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'accesosusu')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('accesosusu'
           ,'accessByEmployee')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'costcenter')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('costcenter'
           ,'ActualCostCenter')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'costjusti')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('costjusti'
           ,'CostCenterAndCauseByEmployee')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'justusu')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('justusu'
           ,'causesByEmployeeDate')


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'planif')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('planif'
           ,'scheduleByEmployeeDate')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'punches')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('punches'
           ,'punches')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'requests')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('requests'
           ,'requestsByEmployee')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'saldes')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('saldes'
           ,'conceptsByEmployeeDate')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'saldpunch')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('saldpunch'
           ,'conceptsColumns')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'tasks')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('tasks'
           ,'tasksByEmployee')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 AND Class = 'justusu')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'justusu')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 AND Class = 'planif')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (7
           ,'planif')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 8 AND Class = 'accesos')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (9
           ,'accesos')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 8 AND Class = 'accesosusu')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (9
           ,'accesosusu')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 AND Class = 'accesosusu')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'accesosusu')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 4 AND Class = 'costcenter')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (4
           ,'costcenter')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 4 AND Class = 'costjusti')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (4
           ,'costjusti')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 AND Class = 'costjusti')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'costjusti')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 AND Class = 'punches')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'punches')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 AND Class = 'saldpunch')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'saldpunch')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 AND Class = 'saldpunch')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'saldpunch')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 8 AND Class = 'tasks')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (8
           ,'tasks')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 AND Class = 'saldes')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'saldes')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 5 AND Class = 'requests')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (5
           ,'requests')
GO

IF EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 AND Class = 'justusu')
DELETE FROM [dbo].[GeniusCustomReportCombs]
      WHERE IdCheck = 7 AND Class = 'justusu'
GO

IF EXISTS
(
    SELECT *
	FROM [dbo].[sysroGUI_Actions]
	WHERE IDPath = 'NewGenius'
)
    BEGIN
		DELETE FROM [dbo].[sysroGUI_Actions]
		WHERE IDPath = 'NewGenius'
	END;
GO

 ALTER PROCEDURE [dbo].[Genius_CostCenters_Detail]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max)   AS  
  		DECLARE @employeeIDs Table(idEmployee int)  
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
  		DECLARE @businessCenterIDs Table(idBusinessCenter int)  
  		insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
  		SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                      dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date, dbo.DailyCauses.Date AS Date_ToDateString,  
                      dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
  					isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
  					isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                      (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,   
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                      dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 0 AS CostCenterTotalCost,   
  					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
  					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,  
  					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyCauses.Date) As UserField1,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyCauses.Date) As UserField2,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyCauses.Date) As UserField3,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyCauses.Date) As UserField4,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyCauses.Date) As UserField5,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyCauses.Date) As UserField6,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyCauses.Date) As UserField7,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyCauses.Date) As UserField8,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyCauses.Date) As UserField9,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyCauses.Date) As UserField10,
 					 dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
           FROM		 dbo.sysroEmployeeGroups with (nolock)   
                          INNER JOIN dbo.Causes with (nolock)   
                          INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   
  						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
  						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
  						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
  						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
            where dbo.dailycauses.date between @pinitialDate and @pendDate 
  					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
  					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

ALTER PROCEDURE [dbo].[Genius_Access]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS  
  		DECLARE @employeeIDs Table(idEmployee int)  
  		DECLARE @zonesIDs Table(idZone int)  
  		DECLARE @idParentPassport int  
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter 
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @pinitialDate, @pendDate;  
  		select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
  		if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')    
  			insert into @zonesIDs select ID from Zones  
  				where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock) where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null  
  		else    
  			insert into @zonesIDs select ID from Zones with (nolock)  
  		SELECT     dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8),   
                      dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day, MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Year,   
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName,   
                      dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName, dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL   
                      THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails) AS Details, dbo.sysroEmployeeGroups.Path,   
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup,  
  					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,                    	
                    dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
 					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
           FROM         dbo.sysroEmployeeGroups with (nolock)   
               INNER JOIN dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee   
               INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate   
               LEFT OUTER JOIN dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID   
               LEFT OUTER JOIN dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID   
               LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
			   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
           WHERE     dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,dbo.Punches.ShiftDate) > 1  
  				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
  				AND (dbo.Punches.ShiftDate between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate)  
  				AND dbo.Zones.ID IN (SELECT idZone FROM @zonesIDs)  
           GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
                      dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
                      dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
                      (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
                      CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
                      dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,   
                      dbo.sysroEmployeeGroups.IDGroup,dbo.EmployeeContracts.BeginDate , dbo.EmployeeContracts.EndDate, dbo.sysrovwEmployeeLockDate.LockDate  
           HAVING (dbo.Punches.Type = 5) OR (dbo.Punches.Type = 6) OR (dbo.Punches.Type = 7)

GO

UPDATE [dbo].[GeniusViews]
   SET [CubeLayout] = '{"slice":{"reportFilters":[{"uniqueName":"CurrentEmployee"},{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"InvalidDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T11:13:12.165Z"}'
 WHERE Name = 'accessByEmployee' and IdPassport = 0
GO

ALTER PROCEDURE [dbo].[Genius_Requests_CostCenters]
        	@initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS
  		DECLARE @employeeIDs Table(idEmployee int)
   		DECLARE @SecurityMode int
   		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
  		
  		SELECT     dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                               dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString', 
                               dbo.Requests.Status AS RequestsStatus, 
    						   	Case 
    							WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )
    							ELSE '---'
    							END as 'ApproveRefusePassport',
    							Case 
    								WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))
    								ELSE '---'
    							END as 'PendingPassport',
    							dbo.Requests.RequestType,
    							isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'PunchDate',
    							ISNULL((SELECT Name from dbo.BusinessCenters WHERE ID = dbo.Requests.IDCenter),'') as 'NameCenter',
    							CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,
                               CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day, 
                               MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1) 
                               % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, 
                               dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, 
        					   dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @endDate AS EndPeriod, @endDate AS EndPeriod_ToDateString,
        						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
        						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
        						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
        						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
        						dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,
        						dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
 							dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, 
							dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
    									
         FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN
                               dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                               dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                               dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
							   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
         WHERE	dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1
        			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
    				
         GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID, dbo.requests.IDCenter ,
                               dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108), 
                               dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                               (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path, 
                               dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
    							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.sysrovwEmployeeLockDate.LockDate
         HAVING      (dbo.Requests.RequestType IN(12))


GO 

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"},{"uniqueName":"IncidenceName"},{"uniqueName":"ZoneTime"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-19T15:45:54.171Z"}'
 WHERE Name = 'Justificaciones por usuario y fecha' and IdPassport = 0

GO

ALTER PROCEDURE [dbo].[Genius_Punches]
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
          SELECT     dbo.Punches.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,   
    					dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Punches.ActualType AS PunchDirection,   
    					dbo.Punches.TypeData AS PunchIDTypeData, CASE WHEN dbo.Punches.TypeData <> 0 THEN dbo.Causes.Name ELSE '' END AS PunchTypeData,   
    					dbo.Punches.DateTime, dbo.Punches.ShiftDate AS Date, dbo.Punches.ShiftDate AS Date_ToDateString, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108) AS Time, DAY(dbo.Punches.ShiftDate) AS Day,   
    					MONTH(dbo.Punches.ShiftDate) AS Month, YEAR(dbo.Punches.ShiftDate) AS Año, (DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1)   
    					% 7 + 1 AS DayOfWeek, 1 AS Value, dbo.Punches.IDZone, dbo.Zones.Name AS ZoneName, dbo.Punches.IDTerminal, dbo.Terminals.Description AS TerminalName,   
    					dbo.Punches.InvalidType, CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END AS IsInvalid, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails)   
    					AS Details, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,
    					dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, dbo.Punches.IsNotReliable, dbo.Punches.Location AS PunchLocation,   
    					dbo.Punches.LocationZone AS PunchLocationZone, dbo.Punches.FullAddress , dbo.Punches.TimeZone AS PunchTimeZone, dbo.sysroPassports.Name AS EditorName,  
    					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
    					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.Punches.ShiftDate) As UserField1,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.Punches.ShiftDate) As UserField2,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.Punches.ShiftDate) As UserField3,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.Punches.ShiftDate) As UserField4,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.Punches.ShiftDate) As UserField5,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.Punches.ShiftDate) As UserField6,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.Punches.ShiftDate) As UserField7,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.Punches.ShiftDate) As UserField8,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.Punches.ShiftDate) As UserField9,  
    					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.Punches.ShiftDate) As UserField10,
 					dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
					dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN  
    					dbo.Punches with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Punches.IDEmployee AND dbo.Punches.ShiftDate >= dbo.sysroEmployeeGroups.BeginDate AND   
    					dbo.Punches.ShiftDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN  
    					dbo.EmployeeContracts with (nolock) ON dbo.Punches.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.Punches.ShiftDate >= dbo.EmployeeContracts.BeginDate AND   
    					dbo.Punches.ShiftDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN  
    					dbo.Zones with (nolock) ON dbo.Punches.IDZone = dbo.Zones.ID LEFT OUTER JOIN  
    					dbo.Terminals with (nolock) ON dbo.Punches.IDTerminal = dbo.Terminals.ID LEFT OUTER JOIN  
    					dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID LEFT OUTER JOIN  
    					dbo.sysroPassports with (nolock) ON dbo.Punches.IDPassport = dbo.sysroPassports.ID LEFT OUTER JOIN  
    					dbo.Causes with (nolock) ON dbo.Punches.TypeData = dbo.Causes.ID  
						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,2,0,0,dbo.Punches.ShiftDate) > 1  
    					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.Punches.ShiftDate between @pinitialDate and @pendDate  
          GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.Punches.ShiftDate), YEAR(dbo.Punches.ShiftDate), dbo.Punches.ID,   
    					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.Punches.ShiftDate), dbo.Punches.ShiftDate, CONVERT(nvarchar(8), dbo.Punches.DateTime, 108),   
    					dbo.Punches.DateTime, dbo.Punches.IDZone, dbo.Punches.IDTerminal, dbo.Punches.InvalidType, dbo.EmployeeContracts.IDContract, dbo.Employees.Name,   
    					(DATEPART(dw, dbo.Punches.ShiftDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.Punches.Type, dbo.Terminals.Description, dbo.Zones.Name,   
    					CASE WHEN dbo.Punches.InvalidType IS NULL THEN 0 ELSE 1 END, CONVERT(nvarchar(MAX), dbo.Punches.TypeDetails), dbo.sysroEmployeeGroups.Path,   
    					dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,   
    					dbo.Punches.IsNotReliable, dbo.Punches.Location, dbo.Punches.LocationZone, dbo.Punches.FullAddress, dbo.Punches.TimeZone, dbo.sysroPassports.Name, dbo.Punches.ActualType,   
    					dbo.Causes.Name, dbo.Punches.TypeData, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate
          HAVING (dbo.Punches.Type = 1) OR  
    					(dbo.Punches.Type = 2) OR  
    					(dbo.Punches.Type = 3) OR  
    					(dbo.Punches.Type = 7 AND (dbo.Punches.ActualType = 1 OR dbo.Punches.ActualType = 2))

GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"PDDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"sorting":{"column":{"type":"asc","tuple":[],"measure":{"uniqueName":"Value","aggregation":"sum"}}},"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:46:46.872Z"}'
 WHERE Name = 'punches' and IdPassport = 0
GO

ALTER PROCEDURE [dbo].[Genius_Requests_Schedule]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS        
  		
  		DECLARE @employeeIDs Table(idEmployee int)        
  		DECLARE @requestTypeFilter Table(idRequest int)        
   		DECLARE @SecurityMode int
   		
  		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
          insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;        
  		insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');         
  		SELECT dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,         
  				dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString',
  				dbo.Requests.Status AS RequestsStatus,         
  				Case WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) ) ELSE '---' END as 'ApproveRefusePassport',        
  				Case WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode)) ELSE '---' END as 'PendingPassport',        
  				dbo.Requests.RequestType, isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest', isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest', 
  				isnull(CONVERT(NVARCHAR(4000), isnull(dbo.Requests.Date2,dbo.Requests.Date1), 120),'') as 'EndDateRequest',        
  				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',        
  				ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',        
  				ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',        
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,        
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')) as FieldNameRequest, 
  				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')) as FieldValueRequest, 
                  CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,         
                  MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
  				1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString,      
                  dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup,         
  				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @endDate AS EndPeriod, @endDate AS EndPeriod_ToDateString,
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,        
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,      
  				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,        
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
 				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,        
				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
          FROM dbo.sysroEmployeeGroups with (nolock) 
  				INNER JOIN dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate 
  				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate 
  				LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
   				left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1        
  				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate        
  				AND dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)      
  		GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,         
  					dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),         
  					dbo.requests.RequestDate, dbo.EmployeeContracts.IDContract,
  					dbo.Employees.Name, (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldName, '')),CONVERT(NVARCHAR(4000),isnull(dbo.Requests.FieldValue, '')), dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), 
  					dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate , aux.NumeroDias, dbo.sysrovwEmployeeLockDate.LockDate
          HAVING  (dbo.Requests.RequestType IN(1,2,3,4,5,6,7,8,9,11,14,15))

GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"TypeDesc"},{"uniqueName":"StatusDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:50:37.290Z"}'
 WHERE Name = 'requestsByEmployee' and IdPassport = 0
GO

ALTER PROCEDURE  [dbo].[Genius_Schedule]
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
   		
   		DECLARE @intdatefirst int
   		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
   		SET DateFirst @intdatefirst;
           insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
           SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
      		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
      		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName,
      		dbo.sysroEmployeesShifts.CurrentDate AS Date, dbo.sysroEmployeesShifts.CurrentDate AS Date_ToDateString, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
      		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
      		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
      		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, DATEPART(QUARTER, dbo.sysroEmployeesShifts.CurrentDate) as Quarter, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
      		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.endDate, dbo.sysroEmployeeGroups.endDate AS endDate_ToDateString,
      		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.endDate AS EndContract, dbo.EmployeeContracts.endDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
      		dbo.sysroEmployeesShifts.IDAssignment,
      		convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark) as Remark,
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
 			dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate, dbo.GetEmployeeAge(sysroEmployeesShifts.idEmployee) As Age, (SELECT LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = sysroEmployeesShifts.IDEmployee) as LockDate_ToDateString
          FROM dbo.sysroEmployeesShifts with (nolock) 
      		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
      		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
			LEFT JOIN Shifts WITH(NOLOCK) ON Shifts.ID = sysroEmployeesShifts.IDShift
      		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
     		LEFT JOIN ProgrammedAbsences WITH(NOLOCK) ON ProgrammedAbsences.IDEmployee = EmployeeContracts.IDEmployee AND CurrentDate BETWEEN ProgrammedAbsences.BeginDate AND isnull(ProgrammedAbsences.FinishDate,DATEADD(day,ProgrammedAbsences.maxlastingdays,ProgrammedAbsences.BeginDate))
      	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
      		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
      	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
      		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
      		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, 
      		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
      		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
      		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
      		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
      		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, convert(nvarchar(max),dbo.sysroEmployeesShifts.Remark)			

GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Hours\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-10-27T07:27:37.854Z"}'
 WHERE Name = 'scheduleByEmployeeDate' and IdPassport = 0
GO

 ALTER PROCEDURE [dbo].[Genius_Requests_Tasks]
        	@initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS
  		DECLARE @employeeIDs Table(idEmployee int)
   		DECLARE @SecurityMode int
   		
  		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
  		
  		SELECT  dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                  dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString', 
                  dbo.Requests.Status AS RequestsStatus, 
    				Case 
    				WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )
    				ELSE '---'
    				END as 'ApproveRefusePassport',
    				Case 
    					WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))
    					ELSE '---'
    				END as 'PendingPassport',
    				dbo.Requests.RequestType,
    				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'DateTask1',
    				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date2, 120),'') as 'DateTask2',
    				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask1),'') as 'NameTask1',
    				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask2),'') as 'NameTask2',
    				isnull(dbo.Requests.CompletedTask,0) as 'CompletedTask',
    				isnull(dbo.Requests.Field1,'') as 'TaskField1',
    				isnull(dbo.Requests.Field2,'') as 'TaskField2',
    				isnull(dbo.Requests.Field3,'') as 'TaskField3',
    				isnull(dbo.Requests.Field4,0) as 'TaskField4',
    				isnull(dbo.Requests.Field5,0) as 'TaskField5',
    				isnull(dbo.Requests.Field6,0) as 'TaskField6',
    				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,
                  CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day, 
                  MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)  % 7 + 1 AS DayOfWeek, 1 AS Value, 
  				dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
        			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
 				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age
    									
         FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN
                               dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                               dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                               dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
         WHERE	dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1
        			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
    				
         GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID, dbo.requests.IDTask1 , dbo.requests.IDTask2,dbo.requests.CompletedTask,dbo.requests.Field1,dbo.requests.Field2,dbo.requests.Field3,dbo.requests.Field4,dbo.requests.Field5,dbo.requests.Field6,
                               dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108), 
                               dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                               (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.Date2,  CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path, 
                               dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
    							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
         HAVING      (dbo.Requests.RequestType IN(10))

GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"TypeDesc"},{"uniqueName":"NameTask1"},{"uniqueName":"NameTask2"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"KeyView","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-22T14:23:17.823Z"}'
 WHERE Name = 'taskRequests' and IdPassport = 0
GO

ALTER PROCEDURE [dbo].[Genius_Tasks]
        @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @userFieldsFilter nvarchar(max) AS  
    		
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
  		SELECT  dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee,   
  					dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) AS Date_Month, 
  					DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.DailyTaskAccruals.Date AS Date_ToDateString, dbo.BusinessCenters.ID AS IDCenter, dbo.BusinessCenters.Name AS CenterName, 
  					dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, 
  					ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task,
  					ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') AS Field3_Total, 
  					ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 0) AS Field5_Total, 
  					ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Value), 0) AS Value, 
  					ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, 
  					ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime,   
  					ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime,
  					ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration,   
                      CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente' END END END AS Estado, 
  					dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  					dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyTaskAccruals.Date) As UserField1,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyTaskAccruals.Date) As UserField2,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyTaskAccruals.Date) As UserField3,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyTaskAccruals.Date) As UserField4,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyTaskAccruals.Date) As UserField5,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyTaskAccruals.Date) As UserField6,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyTaskAccruals.Date) As UserField7,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyTaskAccruals.Date) As UserField8,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyTaskAccruals.Date) As UserField9,  
  					dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyTaskAccruals.Date) As UserField10,
 			        dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
					dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
  		FROM    dbo.sysroEmployeeGroups with (nolock)    
                      INNER JOIN dbo.DailyTaskAccruals with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
                      INNER JOIN dbo.Tasks with (nolock) ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask   
                      INNER JOIN dbo.BusinessCenters with (nolock) ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID   
                      INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate  
  					INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
					LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
         WHERE    (dbo.businesscenters.id is null OR dbo.BusinessCenters.ID IN (SELECT DISTINCT IDCenter FROM sysroPassports_Centers with (nolock) WHERE IDPassport=(SELECT isnull(IdparentPassport,0) from sysroPassports where ID = @pidpassport)) )  
  					AND dbo.DailyTaskAccruals.Date between @pinitialDate and @pendDate



GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"CenterName"},{"uniqueName":"Project"},{"uniqueName":"TaskName"},{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"Date_ToDateString"}],"measures":[{"uniqueName":"Value_ToHours","formula":"sum(\"Value\")","caption":"Value_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"rows"}},"creationDate":"2021-04-22T14:19:18.308Z"}'
 WHERE Name = 'tasksByEmployee' and IdPassport = 0
GO

ALTER PROCEDURE [dbo].[Genius_CostCenters]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @costCenterFilter nvarchar(max) AS
 		DECLARE @businessCenterIDs Table(idBusinessCenter int)
 		declare @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
  	   	DECLARE @intdatefirst int
 		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'
  		SET DateFirst @intdatefirst;
 		insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');
         SELECT CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date, dbo.DailyCauses.Date AS Date_ToDateString,
                 dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter,
 				isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,
 				isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,
                 (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,
 				dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,
 				dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,
 				0 AS CostCenterTotalCost
           FROM  dbo.sysroEmployeeGroups with (nolock)
                 INNER JOIN dbo.Causes with (nolock)
                 INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate
 				INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID
 				LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID
 				LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON isnull(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID
            where     isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)
   					-- and isnull(dbo.DailyCauses.IDCenter,0) in (select idcenter from sysroPassports_Centers where IDPassport = (select idparentpassport from sysroPassports where id = @pidpassport))
 					and dbo.dailycauses.date between @pinitialDate and @pendDate

GO

ALTER PROCEDURE [dbo].[Genius_CostCenters_ActualStatus]
          @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max) AS  
  	 DECLARE @employeeIDs Table(idEmployee int)  
       DECLARE @pinitialDate smalldatetime = getdate(), @pendDate smalldatetime = getdate(), @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
  	 insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
       DECLARE @businessCenterIDs Table(idBusinessCenter int)  
       insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
  	SELECT     CONVERT(varchar(10), dbo.sysrovwEmployeeStatus.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract  AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                  dbo.EmployeeContracts.IDContract, dbo.sysrovwEmployeeStatus.IDEmployee, dbo.sysrovwEmployeeStatus.CostDatetime, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
  				isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
  				isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5,
                  dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                  dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,   
                  dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
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
  				dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10,
 				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  
           FROM   dbo.sysroEmployeeGroups with (nolock)   
                      INNER JOIN dbo.sysrovwEmployeeStatus with (nolock) ON  dbo.sysroEmployeeGroups.IDEmployee = dbo.sysrovwEmployeeStatus.IDEmployee AND GETDATE() between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   
  					INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.EmployeeContracts.IDEmployee AND GETDATE() >= dbo.EmployeeContracts.BeginDate AND GETDATE() <= dbo.EmployeeContracts.EndDate   
  					INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
  					INNER JOIN dbo.Employees with (nolock) ON dbo.sysrovwEmployeeStatus.IDEmployee = dbo.Employees.ID    
  					LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.sysrovwEmployeeStatus.CostIDCenter,0) = dbo.BusinessCenters.ID   
					LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          where   dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysrovwEmployeeStatus.IDEmployee,2,0,0,getdate()) > 1 
  				AND isnull(dbo.sysrovwEmployeeStatus.CostIDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
  				AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

 ALTER PROCEDURE [dbo].[Genius_Authorizations]
         @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max) AS  
  		DECLARE @employeeIDs Table(idEmployee int)  
  		DECLARE @autorizationsIDs Table(idAccess int)  
  		DECLARE @zonesIDs Table(idZone int)  
  		DECLARE @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter
  		DECLARE @cDate as Date, @idParentPassport int  
  		
  		select @idParentPassport = IDParentPassport from sysroPassports where id = @pidpassport  
  		if ((select value from sysroLiveAdvancedParameters where ParameterName = 'AccessGroupsMode') = '1')  
  		begin  
  			insert into @autorizationsIDs select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where IDPassport = @idParentPassport  
  			insert into @zonesIDs select ID from Zones  
  				where ID in (SELECT IDZone FROM AccessGroupsPermissions with (nolock) where IDAccessGroup in (select IDAccessGroup from sysroPassports_AccessGroup with (nolock) where idpassport = @idParentPassport)) or IDParent is null  
  		end  
  		else  
  		BEGIN  
  			insert into @autorizationsIDs select id from AccessGroups with (nolock)  
  			insert into @zonesIDs select ID from Zones with (nolock)  
  		END  
  		SET @cDate = GETDATE()  
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter, @cDate, @cDate;  
  		SELECT dbo.sysroEmployeeGroups.IDEmployee AS IDEmployee, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.IDGroup AS IDGroup, dbo.sysroEmployeeGroups.FullGroupName As FullGroupName, dbo.sysroEmployeeGroups.Path AS GroupPath,  
  				dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract AS IDContract, dbo.AccessGroups.Name AS AuthorizationName,   
  				Zones.Name AS ZoneName, zones.IsWorkingZone AS IsWorkingZone, AccessPeriods.Name As AccessPeriodName, 1 AS BelongsToGroup,  
  		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,  
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
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),getdate()) As UserField10,
 		dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age   
  			FROM dbo.sysroEmployeeGroups with (nolock)   
  			INNER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID  
  			INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee and getdate() between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
  			INNER JOIN dbo.AccessGroups with (nolock) ON dbo.AccessGroups.ID IN  
  							(SELECT        dbo.Employees.IDAccessGroup  
  							UNION  
  							SELECT        IDAuthorization  
  							FROM            dbo.sysrovwAccessAuthorizations with (nolock)  
  							WHERE        (IDEmployee = dbo.Employees.ID))  
  			INNER JOIN dbo.AccessGroupsPermissions with (nolock) ON AccessGroupsPermissions.IDAccessGroup = AccessGroups.ID  
  			INNER JOIN dbo.AccessPeriods with (nolock) ON AccessPeriods.ID = AccessGroupsPermissions.IDAccessPeriod  
  			INNER JOIN dbo.Zones with (nolock) on Zones.ID = AccessGroupsPermissions.IDZone  
			LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
  		WHERE dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeeGroups.IDEmployee,9,0,0,getdate()) > 1  
  			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) AND getdate() between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate  
  			AND dbo.AccessGroups.ID IN (SELECT idAccess FROM @autorizationsIDs)  
  			AND dbo.Zones.ID in (select idZone from @zonesIDs)

GO

ALTER PROCEDURE [dbo].[Genius_CostCenters_Detail]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @costCenterFilter nvarchar(max)   AS  
  		DECLARE @employeeIDs Table(idEmployee int)  
  		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pbusinessCenterFilter nvarchar(max) = @costCenterFilter
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;  
  		DECLARE @businessCenterIDs Table(idBusinessCenter int)  
  		insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
  		SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,   
                      dbo.EmployeeContracts.IDContract, dbo.DailyCauses.IDEmployee, dbo.DailyCauses.Date, dbo.DailyCauses.Date AS Date_ToDateString,  
                      dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor, isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
  					isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
  					isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                      (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy, dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter,   
                      dbo.Employees.Name AS EmployeeName, dbo.Groups.Name AS GroupName, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName, dbo.Groups.Path,   
                      dbo.sysroEmployeeGroups.BeginDate,dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee, 0 AS CostCenterTotalCost,   
  					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
  					dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP,  
  					dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod,  @pendDate AS EndPeriod_ToDateString,
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
  					 dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailyCauses.Date) As UserField1,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailyCauses.Date) As UserField2,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailyCauses.Date) As UserField3,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailyCauses.Date) As UserField4,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailyCauses.Date) As UserField5,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailyCauses.Date) As UserField6,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailyCauses.Date) As UserField7,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailyCauses.Date) As UserField8,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailyCauses.Date) As UserField9,  
  					 dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailyCauses.Date) As UserField10,
 					 dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age  ,
					 dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
           FROM		 dbo.sysroEmployeeGroups with (nolock)   
                          INNER JOIN dbo.Causes with (nolock)   
                          INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate   
  						INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
  						INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
  						LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
  						LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON ISNULL(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
						LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
            where dbo.dailycauses.date between @pinitialDate and @pendDate 
  					AND isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
  					AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs)

GO

ALTER PROCEDURE [dbo].[Genius_SalaryGap]    
          @initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS    
      DECLARE @employeeIDs Table(idEmployee int)    
      DECLARE @pinitialDate smalldatetime = @initialDate  
      DECLARE @pendDate smalldatetime = @endDate  
         DECLARE @pidpassport int = @idpassport  
         DECLARE @pemployeeFilter nvarchar(max) = @employeeFilter  
         DECLARE @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
         insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate    
  	 select salaryGap.*,
  		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_man,
  		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroBaseSalary ELSE NULL END as sysroBaseSalary_woman,
 		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_man,
  		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroEarningsOverTime ELSE NULL END as sysroEarningsOverTime_woman,
 		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_man,
  		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroExtraSalary ELSE NULL END as sysroExtraSalary_woman,
 		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_man,
  		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroTotalSalary ELSE NULL END as sysroTotalSalary_woman,
 		CASE WHEN salaryGap.sysroGender = 'Hombre' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_man,
  		CASE WHEN salaryGap.sysroGender = 'Mujer' THEN salaryGap.sysroSalarySupp ELSE NULL END as sysroSalarySupp_woman
  	 from(
         SELECT emp.ID As IDEmployee, sreg.IDGroup, ec.IDContract,  
           sreg.GroupName, sreg.FullGroupName, emp.Name AS EmployeeName,     
           sreg.Path, sreg.CurrentEmployee, sreg.BeginDate, sreg.BeginDate AS BeginDate_ToDateString, sreg.EndDate, sreg.EndDate AS EndDate_ToDateString,
           ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,    
           convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroBaseSalary'),@pendDate)) As sysroBaseSalary,
 		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroEarningsOverTime'),@pendDate)) As sysroEarningsOverTime,
 		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroExtraSalary'),@pendDate)) As sysroExtraSalary,
 		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroTotalSalary'),@pendDate)) As sysroTotalSalary,
 		  convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroSalarySupp'),@pendDate)) As sysroSalarySupp,
           convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroProfessionalCategory'),@pendDate)) As sysroProfessionalCategory,    
           convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPosition'),@pendDate)) As sysroPosition,    
           convert(nvarchar(max),dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,(SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroGender'),@pendDate)) As sysroGender,
 		  dbo.GetEmployeeAge(sreg.idEmployee) As Age,  
           dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,2,'\') As Nivel2,    
           dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,4,'\') As Nivel4,    
           dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,6,'\') As Nivel6,    
           dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,8,'\') As Nivel8,    
           dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(sreg.FullGroupName,10,'\') As Nivel10,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),@pendDate) As UserField1,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),@pendDate) As UserField2,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),@pendDate) As UserField3,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),@pendDate) As UserField4,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),@pendDate) As UserField5,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),@pendDate) As UserField6,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),@pendDate) As UserField7,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),@pendDate) As UserField8,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),@pendDate) As UserField9,    
           dbo.GetEmployeeUserFieldValueMin(sreg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),@pendDate) As UserField10    ,
		   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
         FROM   
    dbo.sysroEmployeeGroups as sreg with (nolock)  
     INNER JOIN dbo.Employees emp  with (nolock) on sreg.IDEmployee = emp.ID  
           INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = emp.ID AND @pendDate between ec.BeginDate and ec.EndDate    
		   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
         WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,1,0,0,@pendDate) > 1  
        AND sreg.IDEmployee in( select idEmployee from @employeeIDs) and CurrentEmployee = 1 and @pendDate between sreg.BeginDate and sreg.EndDate) salaryGap

GO

ALTER PROCEDURE [dbo].[Genius_Incidences]
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
     DECLARE @intdatefirst int  
     SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
     SET DateFirst @intdatefirst;  
     insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;       
           SELECT     CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                                 + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120) AS KeyView, dbo.sysroEmployeeGroups.IDGroup,     
                                 dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.DailySchedule.Date AS Date_ToDateString, dbo.sysroDailyIncidencesDescription.Description AS IncidenceName,     
                                 dbo.TimeZones.Name AS ZoneTime, dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS Value, SUM(dbo.DailyCauses.Value) AS ValueHHMM_ToHours, MONTH(dbo.DailySchedule.Date) AS Mes, YEAR(dbo.DailySchedule.Date) AS Año,     
                                 (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, dbo.DailySchedule.Date) AS WeekOfYear, DATEPART(dy, dbo.DailySchedule.Date) AS DayOfYear, 
                                 DATEPART(QUARTER, dbo.DailySchedule.Date) as Quarter, CASE WHEN DailySchedule.Date <= GETDATE() THEN Shifts.Name ELSE Shifts_1.Name END AS ShiftName,     
                                 dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeeGroups.GroupName AS GroupName, dbo.sysroEmployeeGroups.FullGroupName AS FullGroupName, dbo.sysroEmployeeGroups.Path,     
                                 dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.CurrentEmployee AS CurrentEmployee,    
                dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @pinitialDate AS BeginPeriod, @pinitialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, dbo.DailySchedule.IDAssignment,    
             case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,    
                                (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,    
                                (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,    
                dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,    
                dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,    
                dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,    
                dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,    
                dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.DailySchedule.Date) As UserField1,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.DailySchedule.Date) As UserField2,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.DailySchedule.Date) As UserField3,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.DailySchedule.Date) As UserField4,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.DailySchedule.Date) As UserField5,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.DailySchedule.Date) As UserField6,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.DailySchedule.Date) As UserField7,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.DailySchedule.Date) As UserField8,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.DailySchedule.Date) As UserField9,    
                dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.DailySchedule.Date) As UserField10,
 			   dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age, convert(nvarchar(max),dbo.sysroRemarks.Text) as Remark,
			   dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString,
			   dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches
           FROM         dbo.sysroEmployeeGroups with (nolock)    
               INNER JOIN dbo.Causes with (nolock)    
               INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause     
               INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailySchedule.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
               INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailySchedule.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date between dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate     
               LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription with (nolock)    
               INNER JOIN dbo.DailyIncidences with (nolock)    
               INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID ON dbo.sysroDailyIncidencesDescription.IDIncidence = dbo.DailyIncidences.IDType ON dbo.DailyCauses.IDEmployee = dbo.DailyIncidences.IDEmployee AND dbo.DailyCauses.Date = dbo.DailyIncidences.Date AND dbo.DailyCauses.IDRelatedIncidence = dbo.DailyIncidences.ID AND dbo.DailyIncidences.Date  between dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate    
               LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID     
               LEFT OUTER JOIN dbo.Shifts AS Shifts_1 with (nolock) ON dbo.DailySchedule.IDShift1 = Shifts_1.ID    
               LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID    
            LEFT OUTER JOIN dbo.sysroRemarks on dbo.sysroRemarks.ID = dbo.DailySchedule.Remarks    
			LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
          WHERE dbo.WebLogin_GetPermissionOverEmployee(@idpassport,dbo.DailyCauses.IDEmployee,2,0,0,dbo.DailyCauses.date) > 1    
             AND dbo.sysroEmployeeGroups.IDEmployee in(select idEmployee from @employeeIDs)     
            and dbo.dailyCauses.Date between @pinitialDate and @pendDate    
           GROUP BY dbo.sysroEmployeeGroups.IDEmployee, dbo.DailySchedule.Date, dbo.sysroDailyIncidencesDescription.Description, dbo.TimeZones.Name, dbo.Causes.Name,     
                                 YEAR(dbo.DailySchedule.Date), CONVERT(varchar(10), dbo.sysroEmployeeGroups.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)     
                     + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailySchedule.Date, 120), CASE WHEN DailySchedule.Date <= GETDATE()     
                                 THEN Shifts.Name ELSE Shifts_1.Name END, dbo.Employees.Name, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, DATEPART(iso_week,     
                                 dbo.DailySchedule.Date), DATEPART(dy, dbo.DailySchedule.Date), (DATEPART(dw, dbo.DailySchedule.Date) + @@DATEFIRST - 1 - 1) % 7 + 1,     
                                 dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,     
                                 dbo.sysroEmployeeGroups.CurrentEmployee, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.DailySchedule.IDAssignment, dbo.DailySchedule.IDDailyBudgetPosition, convert(nvarchar(max),dbo.sysroRemarks.Text),
								 dbo.sysrovwEmployeeLockDate.LockDate,
								 dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date)

GO

ALTER PROCEDURE [dbo].[Genius_Concepts]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS  
 		DECLARE @employeeIDs Table(idEmployee int)  
 		DECLARE @conceptIDs Table(idConcept int)  
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros  
 		DECLARE @intdatefirst int  
 		SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
 		SET DateFirst @intdatefirst;  
 		
 		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate  
 		insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');  
 		
 		IF @pincludeZeros = 1   
 		BEGIN  
 			WITH alldays AS (  
 				SELECT @initialDate AS dt  
 				UNION ALL  
 				SELECT DATEADD(dd, 1, dt)  
 				FROM alldays s  
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT   
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, sum(isnull(dbo.DailyAccruals.Value,0)) AS ValueHHMM_ToHours,COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
 			FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = reqReg.idEmployee
 			GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
 				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
 				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
 				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate
 			option (maxrecursion 0)  
 		END  
 		ELSE  
 		BEGIN  
 			WITH alldays AS (  
 				SELECT @initialDate AS dt  
 				UNION ALL  
 				SELECT DATEADD(dd, 1, dt)  
 				FROM alldays s  
 				WHERE DATEADD(dd, 1, dt) <= @pendDate)  
 			SELECT
 				reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
 				reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, sum(isnull(dbo.DailyAccruals.Value,0)) AS Value, COUNT(*) AS Count, MONTH(reqReg.dt) AS Mes,   
 				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week,   
 				reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
 				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate,  dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
 				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod,  @pendDate AS EndPeriod_ToDateString,
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,  
 				dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
 				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10,
				dbo.GetEmployeeAge(reqReg.idEmployee) As Age,
				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
 		   FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
 				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
 				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
 				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
 				) reqReg  
 				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
 				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  
 				INNER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept  
				LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = reqReg.idEmployee
 		   GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName,   
 				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt),   
 				YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy,   
 				reqReg.dt), CAST(reqReg.idEmployee AS varchar) + '-' + CAST(reqReg.idConcept AS varchar)   
 				+ '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) + '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, reqReg.dt, 120),   
 				10), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,  
 				dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate
 		   option (maxrecursion 0)  
        END  

GO

ALTER PROCEDURE [dbo].[GetPivotAccruals]
   @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
 AS
 BEGIN
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
  DECLARE @employeeIDs Table(idEmployee int)
  DECLARE @conceptNames NVARCHAR(MAX)
  DECLARE @employeeIDFilter NVARCHAR(MAX)
  DECLARE @intdatefirst int
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  SET DateFirst @intdatefirst;  
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
  select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from 
 	(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp
  select @conceptNames = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +']' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
   DECLARE @SqlStatement NVARCHAR(MAX)
   SET @SqlStatement = N'
     SELECT 
 		accruals.IDEmployee As IDEmployee, eg.IDGroup, ec.IDContract, eg.GroupName, eg.FullGroupName, emp.Name AS EmployeeName, accruals.Date AS Date_ToDateString,
 		MONTH(accruals.Date) AS Mes, YEAR(accruals.Date) AS Año, (DATEPART(dw, accruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, accruals.Date) AS WeekOfYear, 
 		DATEPART(dy, accruals.Date) AS DayOfYear, DATEPART(QUARTER, accruals.Date) AS Quarter, eg.Path, eg.CurrentEmployee, eg.BeginDate, eg.BeginDate AS BeginDate_ToDateString, eg.EndDate, eg.EndDate AS EndDate_ToDateString, ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString,
 		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,
 		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),accruals.Date) As UserField1,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),accruals.Date) As UserField2,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),accruals.Date) As UserField3,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),accruals.Date) As UserField4,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),accruals.Date) As UserField5,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),accruals.Date) As UserField6,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),accruals.Date) As UserField7,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),accruals.Date) As UserField8,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),accruals.Date) As UserField9,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),accruals.Date) As UserField10,
 		dbo.GetEmployeeAge(emp.ID) As Age,
 		' + @conceptNames + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches,
		dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
 	from (SELECT * FROM
          (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date FROM DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
 		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and ''' + convert(nvarchar(max),@pendDate,112) + ''' 
 		  and IDEmployee IN(' + @employeeIDFilter + ') and dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',IDEmployee,2,0,0,Date) > 1
 		 ) I
          PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) accruals
 	INNER JOIN dbo.Employees emp with (nolock) ON emp.ID = accruals.IDEmployee 
 	INNER JOIN dbo.sysroEmployeeGroups eg with (nolock) ON eg.IDEmployee = accruals.IDEmployee AND accruals.Date between eg.BeginDate and eg.EndDate  
  	INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = accruals.IDEmployee AND accruals.Date between ec.BeginDate and ec.EndDate  
	LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
    ORDER BY accruals.IDEmployee,accruals.Date ASC';
   EXEC(@SqlStatement)
 END

 GO

 ALTER PROCEDURE [dbo].[GetPivotAccrualsWithZeros]
    @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max) 
 AS
 BEGIN
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
  DECLARE @employeeIDs Table(idEmployee int)
  DECLARE @conceptNames NVARCHAR(MAX)
  DECLARE @employeeIDFilter NVARCHAR(MAX)
  DECLARE @intdatefirst int
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS'  
  SET DateFirst @intdatefirst;  
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
  select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from 
 	(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp
  select @conceptNames = substring(tmp.conceptNames,2, LEN(tmp.conceptNames) -1) from 
 	(select convert(nvarchar(max), (SELECT ',[' + ST1.name +']' AS [data()]
                 FROM dbo.concepts ST1 WITH(NOLOCK)
 				where st1.ID in(select Value from dbo.SplitToInt(@pconceptsFilter,','))
                 FOR XML PATH(''))) as conceptNames) tmp
   DECLARE @SqlStatement NVARCHAR(MAX)
   SET @SqlStatement = N'
 	WITH alldays AS (  
            SELECT convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) AS dt  
            UNION ALL  
            SELECT DATEADD(dd, 1, dt)  
           FROM alldays s  
             WHERE DATEADD(dd, 1, dt) <=  convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120))  
 	SELECT 
 		accruals.IDEmployee As IDEmployee, eg.IDGroup, ec.IDContract, eg.GroupName, eg.FullGroupName, emp.Name AS EmployeeName, accruals.Date AS Date_ToDateString,
 		MONTH(accruals.Date) AS Mes, YEAR(accruals.Date) AS Año, (DATEPART(dw, accruals.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, accruals.Date) AS WeekOfYear, 
 		DATEPART(dy, accruals.Date) AS DayOfYear, DATEPART(QUARTER, accruals.Date) AS Quarter, eg.Path, eg.CurrentEmployee, eg.BeginDate, eg.BeginDate AS BeginDate_ToDateString, eg.EndDate, eg.EndDate AS EndDate_ToDateString, ec.BeginDate AS BeginContract, ec.BeginDate AS BeginContract_ToDateString, ec.EndDate AS EndContract, ec.EndDate AS EndContract_ToDateString,
 		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,  
  		dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,
 		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),accruals.Date) As UserField1,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),accruals.Date) As UserField2,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),accruals.Date) As UserField3,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),accruals.Date) As UserField4,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),accruals.Date) As UserField5,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),accruals.Date) As UserField6,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),accruals.Date) As UserField7,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),accruals.Date) As UserField8,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),accruals.Date) As UserField9,  
  		dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),accruals.Date) As UserField10,
 		dbo.GetEmployeeAge(emp.ID) As Age,
 		' + @conceptNames + ', dbo.GetPunchesByEmployeeAndDate(emp.ID,accruals.Date) as punches,
		dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
 	FROM(
     select alldays.dt as Date, employees.ID as IDEmployee, '+ @conceptNames +' from alldays
 	cross join Employees
 	left outer join (SELECT *
       FROM
          (SELECT Concepts.name AS ConceptName, Value, IDEmployee, Date from DailyAccruals WITH(NOLOCK) inner join Concepts WITH(NOLOCK) on dailyaccruals.IDConcept = Concepts.ID
 		  WHERE Date between ''' + convert(nvarchar(max),@pinitialDate,112) + ''' and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) 
 		 ) I
          PIVOT( Sum(Value) FOR ConceptName in ('+ @conceptNames +') ) P) tmp
 		 on alldays.dt = tmp.Date and tmp.IDEmployee = employees.ID
 	 WHERE employees.ID IN(' + @employeeIDFilter + ') and dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',employees.ID,2,0,0,alldays.dt) > 1
 	) accruals
 	INNER JOIN dbo.Employees emp with (nolock) ON emp.ID = accruals.IDEmployee 
 	INNER JOIN dbo.sysroEmployeeGroups eg with (nolock) ON eg.IDEmployee = accruals.IDEmployee AND accruals.Date between eg.BeginDate and eg.EndDate  
  	INNER JOIN dbo.EmployeeContracts ec with (nolock) ON ec.IDEmployee = accruals.IDEmployee AND accruals.Date between ec.BeginDate and ec.EndDate  
	LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
 	ORDER BY accruals.IDEmployee,accruals.Date ASC
 	option ( MaxRecursion 0 ) 
 	';
   EXEC(@SqlStatement)
 END

GO

ALTER PROCEDURE [dbo].[Genius_Requests_Tasks]
        	@initialDate smalldatetime,@endDate smalldatetime,@idpassport int,@employeeFilter nvarchar(max),@userFieldsFilter nvarchar(max) AS
  		DECLARE @employeeIDs Table(idEmployee int)
   		DECLARE @SecurityMode int
   		
  		SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
  		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;
  		
  		SELECT  dbo.requests.ID AS KeyView, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee, 
                  dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date', dbo.Requests.RequestDate as 'Date_ToDateString', 
                  dbo.Requests.Status AS RequestsStatus, 
    				Case 
    				WHEN Requests.Status IN(2,3) THEN (SELECT top 1 isnull(sysropassports.Name,'') from RequestsApprovals inner join sysroPassports on RequestsApprovals.IDPassport = sysroPassports.ID  where IDRequest = Requests.ID AND Status in(2,3) )
    				ELSE '---'
    				END as 'ApproveRefusePassport',
    				Case 
    					WHEN Requests.Status IN(0,1) Then (select dbo.GetRequestNextLevelPassports(Requests.ID,@SecurityMode))
    					ELSE '---'
    				END as 'PendingPassport',
    				dbo.Requests.RequestType,
    				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'DateTask1',
    				isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date2, 120),'') as 'DateTask2',
    				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask1),'') as 'NameTask1',
    				ISNULL((SELECT Name from dbo.Tasks WHERE ID = dbo.Requests.IDTask2),'') as 'NameTask2',
    				isnull(dbo.Requests.CompletedTask,0) as 'CompletedTask',
    				isnull(dbo.Requests.Field1,'') as 'TaskField1',
    				isnull(dbo.Requests.Field2,'') as 'TaskField2',
    				isnull(dbo.Requests.Field3,'') as 'TaskField3',
    				isnull(dbo.Requests.Field4,0) as 'TaskField4',
    				isnull(dbo.Requests.Field5,0) as 'TaskField5',
    				isnull(dbo.Requests.Field6,0) as 'TaskField6',
    				CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,
                  CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day, 
                  MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)  % 7 + 1 AS DayOfWeek, 1 AS Value, 
  				dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, dbo.sysroEmployeeGroups.IDGroup, 
        			dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @endDate AS EndPeriod, @endDate AS EndPeriod_ToDateString,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
        			dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,1,','),dbo.Requests.RequestDate) As UserField1,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,2,','),dbo.Requests.RequestDate) As UserField2,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,3,','),dbo.Requests.RequestDate) As UserField3,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,4,','),dbo.Requests.RequestDate) As UserField4,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,5,','),dbo.Requests.RequestDate) As UserField5,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,6,','),dbo.Requests.RequestDate) As UserField6,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,7,','),dbo.Requests.RequestDate) As UserField7,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,8,','),dbo.Requests.RequestDate) As UserField8,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,9,','),dbo.Requests.RequestDate) As UserField9,
        			dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10,
 				dbo.GetEmployeeAge(sysroEmployeeGroups.idEmployee) As Age,
				dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
    									
         FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN
                               dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN
                               dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND 
                               dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN
                               dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID 
							   LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.Employees.ID
         WHERE	dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1
        			AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate
    				
         GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID, dbo.requests.IDTask1 , dbo.requests.IDTask2,dbo.requests.CompletedTask,dbo.requests.Field1,dbo.requests.Field2,dbo.requests.Field3,dbo.requests.Field4,dbo.requests.Field5,dbo.requests.Field6,
                               dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108), 
                               dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name, 
                               (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.Date2,  CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path, 
                               dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup, 
    							dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate, dbo.sysrovwEmployeeLockDate.LockDate
         HAVING      (dbo.Requests.RequestType IN(10))


GO



ALTER PROCEDURE  [dbo].[Genius_Users]
         @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @requestTypesFilter nvarchar(max) AS      

 		DECLARE @employeeIDs Table(idEmployee int)      
 		DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter       
		DECLARE @employeeIDFilter NVARCHAR(MAX)
		DECLARE @featureNames Table(feature nvarchar(MAX), featureid int, fieldName nvarchar(max), requestTypeId int)
		DECLARE @featureFields NVARCHAR(max)
		DECLARE @securityVersion int = 1

		insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate
		select @employeeIDFilter = substring(tmp.empIds,2, len(tmp.empIds) -1) from
			(select convert(nvarchar(max), (select ',' + convert(nvarchar(max),idemployee) as [data()] from @employeeIDs FOR XML PATH(''))) as empIds) tmp

		IF @requestTypesFilter = ''
			SET @requestTypesFilter = '-1'

		IF EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode')
			select @securityVersion = convert(int,value) from [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'SecurityMode'
			
		
		
		insert into @featureNames exec dbo.ObtainFeaturesFromFilter @requestTypesFilter
		if @securityVersion < 3
		begin
			select @featureFields = isnull(tmp.empIds,'') from
				(select convert(nvarchar(max), (select ',dbo.GetFeatureNextLevelPassportsByEmployee(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
		end
		else
		begin
			select @featureFields = isnull(tmp.empIds,'') from
			(select convert(nvarchar(max), (select ',dbo.GetDirectSupervisorNameByRequest(emp.id,''' + feature  +''',' + convert(nvarchar(max),featureid) + ',' + convert(nvarchar(max),requestTypeId) + ') AS [sup' + fieldName + ']' as [data()] from @featureNames FOR XML PATH(''))) as empIds) tmp
		end
		
		
	DECLARE @SqlStatement NVARCHAR(MAX)
	SET @SqlStatement = N'
 		select * from (  
 			select CAST(emp.ID AS varchar) + ''-'' + CAST(eg.IDGroup AS varchar) + ''-'' + ec.IDContract AS KeyView,
 			emp.Id as IDEmployee, emp.Name as EmployeeName, spam.method as Method, spam.credential as Credential, spam.version as Version,     
 			CASE WHEN len(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX),spam.BiometricData))) > 0 THEN ''X'' ELSE '''' END AS BiometricData,    
 			spam.Enabled as Enabled, spam.TimeStamp as TimeStamp, spam.TimeStamp AS TimeStamp_ToDateString, t.Description as Terminal, spam.BiometricAlgorithm as BiometricAlgorithm,     
 			g.name as GroupName, g.FullGroupName, ec.IDContract, ec.BeginDate as BeginContract, ec.BeginDate as BeginContract_ToDateString, ec.EndDate as EndContract, ec.EndDate as EndContract_ToDateString, eg.BeginDate as BeginDate, eg.BeginDate as BeginDate_ToDateString, eg.EndDate as EndDate, eg.EndDate as EndDate_ToDateString,
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,1,''\'') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,2,''\'') As Nivel2,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,3,''\'') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,4,''\'') As Nivel4,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,5,''\'') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,6,''\'') As Nivel6,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,7,''\'') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,8,''\'') As Nivel8,      
 			dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,9,''\'') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(eg.FullGroupName,10,''\'') As Nivel10,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',1,'',''),GetDate()) As UserField1,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',2,'',''),GetDate()) As UserField2,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',3,'',''),GetDate()) As UserField3,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',4,'',''),GetDate()) As UserField4,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',5,'',''),GetDate()) As UserField5,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',6,'',''),GetDate()) As UserField6,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',7,'',''),GetDate()) As UserField7,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',8,'',''),GetDate()) As UserField8,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',9,'',''),GetDate()) As UserField9,      
 			dbo.GetEmployeeUserFieldValueMin(emp.Id,dbo.UFN_SEPARATES_COLUMNS(''' + @puserFieldsFilter + ''',10,'',''),GetDate()) As UserField10,
			(select top 1 datetime from punches where IdEmployee = emp.Id order by datetime desc) AS [LastPunch_ToDateString],
			dbo.GetEmployeeAge(emp.Id) As Age' + @featureFields + ',    
			dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
         FROM employees emp    
 			 inner join sysroPassports sp on sp.IDEmployee = emp.id    
 			 inner join sysroPassports_AuthenticationMethods spam on spam.IDPassport = sp.ID    
 			 inner join EmployeeContracts ec on emp.id = ec.IDEmployee    
 			 inner join sysroEmployeeGroups eg on eg.IDEmployee = emp.id    
 			 inner join Groups g on g.id = eg.IDGroup    
 			 left join Terminals t on t.id = spam.BiometricTerminalId    
			 LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
 		WHERE dbo.WebLogin_GetPermissionOverEmployee(' + CAST(@pidpassport as nvarchar(100)) + ',emp.Id,2,0,0,GetDate()) > 1      
 			AND emp.Id in(' + @employeeIDFilter + ') and convert(smalldatetime,''' + convert(nvarchar(max),@pinitialDate,112) + ''',120) >= ec.BeginDate and convert(smalldatetime,''' + convert(nvarchar(max),@pendDate,112) + ''',120) <= ec.EndDate  and Version != ''RXA200''  ) as query';
 
	EXEC(@SqlStatement)


GO



GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE IdPassport = -1 AND Name = 'justificationsByUserDate')
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (-1
           ,'justificationsByUserDate'
           ,''
           ,'C'
           ,1
           ,'20211215'
           ,''
           ,0
           ,'20210101'
           ,'20211231'
           ,'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Punches"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"},{"uniqueName":"IncidenceName"},{"uniqueName":"ZoneTime"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Value_ToHours","caption":"Horas", "formula":"sum(\"Value\")"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-19T15:45:54.171Z"}'
           ,''
           ,''
           ,''
           ,'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}'
           ,'Genius_Incidences(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter)'
           ,'Calendar'
           ,'Calendar.Analytics')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'justfich')

INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('justfich'
           ,'justificationsByUserDate')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 and Class = 'justfich')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'justfich')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 and Class = 'justfich')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'justfich')
GO

DROP PROCEDURE IF EXISTS [dbo].[Genius_ConceptsAndScheduleAndIncidences]
GO
CREATE PROCEDURE [dbo].[Genius_ConceptsAndScheduleAndIncidences]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS 
  DECLARE @employeeIDs Table(idEmployee int) 
  DECLARE @conceptIDs Table(idConcept int) 
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros 
  DECLARE @intdatefirst int 
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS' 
  SET DateFirst @intdatefirst;   
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate 
  insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');   
  WITH alldays AS ( 
  SELECT @initialDate AS dt 
  UNION ALL 
  SELECT DATEADD(dd, 1, dt) 
  FROM alldays s 
  WHERE DATEADD(dd, 1, dt) <= @pendDate)
  SELECT  
  reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,  
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,  
  reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, isnull(dbo.DailyAccruals.Value,0) AS Value, isnull(dbo.DailyAccruals.Value,0) AS ValueHHMM_ToHours,COUNT(*) AS Count,  
  MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,  
  dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, 
  dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
  dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName, did.Description AS IncidenceName, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10, 
  dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
  dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString 
  FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
  (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
  (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
  where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
  ) reqReg 
  INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
  INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
  INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
  INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
  INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
  LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
  LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept 
  LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) ) 
  LEFT OUTER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
  LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date 
  LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType 
  LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID     
  GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
  YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
  dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
  dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
  dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name

  UNION

  SELECT 
  emp.ID As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, NULL AS IDConcept, 
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name AS EmployeeName, 
  '' AS ConceptName, ds.Date AS Date, ds.Date AS Date_ToDateString, 0 AS Value, 0 AS ValueHHMM_ToHours,COUNT(*) AS Count, MONTH(ds.Date) AS Mes, 
  YEAR(ds.Date) AS Año, (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, 
  ds.Date) AS WeekOfYear, DATEPART(dy, ds.Date) AS DayOfYear, DATEPART(QUARTER, ds.Date) AS Quarter, dbo.sysroEmployeeGroups.Path, 
  dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
  dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
  dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName,
  did.Description AS IncidenceName,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),ds.Date) As UserField1,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),ds.Date) As UserField2,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),ds.Date) As UserField3,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),ds.Date) As UserField4,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),ds.Date) As UserField5,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),ds.Date) As UserField6,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),ds.Date) As UserField7,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),ds.Date) As UserField8,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),ds.Date) As UserField9,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),ds.Date) As UserField10,
  dbo.GetEmployeeAge(emp.ID) As Age     ,
  dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
               FROM dbo.DailySchedule ds with (nolock)
                                            INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
            INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
                                            LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses))
                                            INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
                                            INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                            INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                            INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                            LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                            LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
               WHERE ds.Date between @pinitialDate and @pendDate AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,ds.Date) > 1
			   and emp.ID in (select idEmployee from @employeeIDs)
                GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
                             dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date), 
                              YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy, 
                              ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
                              dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
                              dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name
               option (maxrecursion 0)

GO

DELETE FROM [dbo].[GeniusViews] WHERE Name = 'scheduleCausesAndConcepts' and IdPassport = -1
IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE Name = 'scheduleCausesAndConcepts' and IdPassport = -1)
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (-1,	'scheduleCausesAndConcepts', '','S',	1,	'20211215',''	,	0,	'20210601',	'20211215',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"IncidenceName"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"ShiftName"},{"uniqueName":"ConceptName"},{"uniqueName":"ValueHHMM_ToHours"}],"columns":[{"uniqueName":"CauseName"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"CauseValue\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2022-01-20T09:37:50.150Z"}'		,'','','',		'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_ConceptsAndScheduleAndIncidences(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros))',	'Calendar',	'Calendar.Analytics')

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'hojusa')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('hojusa'
           ,'scheduleCausesAndConcepts')
GO


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 and Class = 'hojusa')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'hojusa')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 and Class = 'hojusa')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'hojusa')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 and Class = 'hojusa')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (7
           ,'hojusa')
GO

DROP PROCEDURE IF EXISTS [Genius_ConceptsAndScheduleAndIncidencesAndPunches]

GO

CREATE PROCEDURE [dbo].[Genius_ConceptsAndScheduleAndIncidencesAndPunches]
          @initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS 
  DECLARE @employeeIDs Table(idEmployee int) 
  DECLARE @conceptIDs Table(idConcept int) 
  DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros 
  DECLARE @intdatefirst int 
  SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value('(/roCollection/Item[@key=("WeekPeriod")]/text())[1]', 'nvarchar(max)') FROM sysroParameters WHERE ID = 'OPTIONS' 
  SET DateFirst @intdatefirst;   
  insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate 
  insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,',');   
  WITH alldays AS ( 
  SELECT @initialDate AS dt 
  UNION ALL 
  SELECT DATEADD(dd, 1, dt) 
  FROM alldays s 
  WHERE DATEADD(dd, 1, dt) <= @pendDate)
  SELECT  
  reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,  
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,  
  reqReg.conceptName AS ConceptName, reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, isnull(dbo.DailyAccruals.Value,0) AS Value, isnull(dbo.DailyAccruals.Value,0) AS ValueHHMM_ToHours,COUNT(*) AS Count,  
  MONTH(reqReg.dt) AS Mes, YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, reqReg.dt) AS WeekOfYear, DATEPART(dy, reqReg.dt) AS DayOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter,  
  dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString, 
  dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString, 
  dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName, did.Description AS IncidenceName, 
  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date) as Punches,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6, 
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9, 
  dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10, 
  dbo.GetEmployeeAge(reqReg.idEmployee) As Age     ,
  dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString 
  FROM (select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,  
  (select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp, 
  (select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con
  where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1
  ) reqReg 
  INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate 
  INNER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee 
  INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
  INNER JOIN dbo.DailySchedule with (nolock) ON dbo.DailySchedule.IDEmployee = reqReg.idEmployee and dbo.DailySchedule.Date = reqReg.dt 
  INNER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(dbo.DailySchedule.IDShiftUsed,dbo.DailySchedule.IDShift1) 
  LEFT OUTER JOIN dbo.DailyAccruals with (nolock) on reqReg.dt = dbo.DailyAccruals.Date and reqReg.idEmployee = dbo.DailyAccruals.IDEmployee and dbo.DailyAccruals.IDConcept = reqReg.idConcept 
  LEFT OUTER JOIN dbo.Concepts with (nolock) ON dbo.Concepts.ID = dbo.DailyAccruals.IDConcept 
  LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.DailySchedule.IDEmployee AND dbo.DailyCauses.Date = dbo.DailySchedule.Date                                                                          AND (DailyCauses.IDCause in ( select IdCause FROM DBO.ConceptCauses with (nolock) where IDConcept = dbo.DailyAccruals.IDConcept) ) 
  LEFT OUTER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
  LEFT OUTER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date 
  LEFT OUTER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType 
  LEFT OUTER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID     
  GROUP BY reqReg.idEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept, reqReg.conceptName, 
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName, reqReg.dt, MONTH(reqReg.dt), 
  YEAR(reqReg.dt), (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, reqReg.dt), DATEPART(dy, reqReg.dt),
  dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
  dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate, dbo.DailyAccruals.Value,
  dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, DailySchedule.Date, Shifts.Name,
  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,dbo.DailySchedule.Date)

  UNION

  SELECT 
  emp.ID As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, NULL AS IDConcept, 
  dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name AS EmployeeName, 
  '' AS ConceptName, ds.Date AS Date, ds.Date AS Date_ToDateString, 0 AS Value, 0 AS ValueHHMM_ToHours,COUNT(*) AS Count, MONTH(ds.Date) AS Mes, 
  YEAR(ds.Date) AS Año, (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(iso_week, 
  ds.Date) AS WeekOfYear, DATEPART(dy, ds.Date) AS DayOfYear, DATEPART(QUARTER, ds.Date) AS Quarter, dbo.sysroEmployeeGroups.Path, 
  dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
  dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
  dbo.Causes.Name AS CauseName, SUM(dbo.DailyCauses.Value) AS CauseValue, SUM(dbo.DailyCauses.Value) AS CauseValueHHMM,Shifts.Name AS ShiftName,
  did.Description AS IncidenceName,
  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,ds.Date) as Punches,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,  
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),ds.Date) As UserField1,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),ds.Date) As UserField2,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),ds.Date) As UserField3,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),ds.Date) As UserField4,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),ds.Date) As UserField5,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),ds.Date) As UserField6,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),ds.Date) As UserField7,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),ds.Date) As UserField8,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),ds.Date) As UserField9,
  dbo.GetEmployeeUserFieldValueMin(emp.ID,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),ds.Date) As UserField10,
  dbo.GetEmployeeAge(emp.ID) As Age     ,
  dbo.sysrovwEmployeeLockDate.LockDate AS LockDate, dbo.sysrovwEmployeeLockDate.LockDate AS LockDate_ToDateString
               FROM dbo.DailySchedule ds with (nolock)
                                            INNER JOIN dbo.Employees emp on ds.IDEmployee = emp.ID
            INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = emp.id
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = emp.ID AND ds.Date between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate
                                            LEFT OUTER JOIN DBO.DailyCauses with (nolock) ON dbo.DailyCauses.IDEmployee = emp.id AND dbo.DailyCauses.Date = ds.Date AND (DailyCauses.IDCause NOT IN (SELECT IDCause FROM DBO.ConceptCauses))
                                            INNER JOIN dbo.Causes with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause
                                            INNER JOIN dbo.DailyIncidences with (nolock) ON dbo.DailyIncidences.id = dbo.DailyCauses.IDRelatedIncidence and dbo.DailyIncidences.IDEmployee = dbo.DailyCauses.IDEmployee and dbo.DailyIncidences.Date = dbo.DailyCauses.Date
                                            INNER JOIN dbo.sysroDailyIncidencesDescription did with (nolock)  on did.IDIncidence = dbo.DailyIncidences.IDType
                                            INNER JOIN dbo.TimeZones with (nolock) ON dbo.DailyIncidences.IDZone = dbo.TimeZones.ID
                                            LEFT OUTER JOIN dbo.Shifts with (nolock) ON dbo.Shifts.ID = isnull(ds.IDShiftUsed,ds.IDShift1)
                                            LEFT OUTER JOIN dbo.sysrovwEmployeeLockDate with (nolock) ON dbo.sysrovwEmployeeLockDate.IDEmployee = emp.ID
               WHERE ds.Date between @pinitialDate and @pendDate AND dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,ds.Date) > 1
			   and emp.ID in (select idEmployee from @employeeIDs)
                GROUP BY emp.ID, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract,
                             dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, emp.Name, ds.date, MONTH(ds.Date), 
                              YEAR(ds.Date), (DATEPART(dw, ds.Date) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(iso_week, ds.Date), DATEPART(dy, 
                              ds.Date), dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate,
                              dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate  , dbo.sysrovwEmployeeLockDate.LockDate,
                              dbo.Causes.Name, dbo.DailyCauses.IDCause, did.Description, ds.Date, Shifts.Name,
							  dbo.GetPunchesByEmployeeAndDate(sysroEmployeeGroups.idEmployee,ds.Date)
               option (maxrecursion 0)



GO

DELETE FROM [dbo].[GeniusViews] WHERE Name = 'scheduleCausesConceptsAndPunches' and IdPassport = -1

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE Name = 'scheduleCausesConceptsAndPunches' and IdPassport = -1)
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (-1,	'scheduleCausesConceptsAndPunches', '','S',	1,	'20211215',''	,	0,	'20210601',	'20211215',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"IncidenceName"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Punches"},{"uniqueName":"ShiftName"},{"uniqueName":"ConceptName"},{"uniqueName":"ValueHHMM_ToHours"}],"columns":[{"uniqueName":"CauseName"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"CauseValue\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2022-01-20T09:37:50.150Z"}'		,'','','',		'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_ConceptsAndScheduleAndIncidencesAndPunches(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros))',	'Calendar',	'Calendar.Analytics')

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'hojusafi')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('hojusafi'
           ,'scheduleCausesConceptsAndPunches')
GO


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 and Class = 'hojusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'hojusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 and Class = 'hojusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'hojusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 and Class = 'hojusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (7
           ,'hojusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 and Class = 'hojusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'hojusafi')
GO

UPDATE [dbo].[GeniusViews]
   SET [CubeLayout] = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"MethodDesc"},{"uniqueName":"Credential"},{"uniqueName":"LastPunch_ToDateString"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T08:07:48.061Z"}'   
 WHERE Name = 'UsersCredentials'
GO

UPDATE [dbo].[GeniusViews]
   SET [CubeLayout] = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"IDContract"},{"uniqueName":"UserField2"},{"uniqueName":"UserField1"},{"uniqueName":"UserField3"},{"uniqueName":"Age"},{"uniqueName":"LastPunch_ToDateString"}],"columns":[{"uniqueName":"[Measures]"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField4","IDContract","UserField2","UserField1","UserField3","Age"]},"options":{"grid":{"type":"flat","showTotals":"off"}},"creationDate":"2021-10-05T14:20:19.691Z"}'   
 WHERE Name = 'usersContract'
GO

UPDATE [dbo].[GeniusViews]
   SET [CubeLayout] = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField5"},{"uniqueName":"UserField1"},{"uniqueName":"Age"},{"uniqueName":"LastPunch_ToDateString"}],"columns":[{"uniqueName":"[Measures]"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField7","UserField6","UserField4","UserField8","UserField1","UserField5","Age"]},"options":{"grid":{"type":"flat","showTotals":"off"}},"creationDate":"2021-09-06T15:22:19.285Z"}'   
 WHERE Name = 'usersWorkCenter'
GO

UPDATE [dbo].[GeniusViews]
   SET [IdPassport] = -1
 where IdPassport = 0 and name not  in ('GapBase','GapExtraSalary','GapPerceptions','GapComplementary','GapTotal','UsersCredentials','usersContract','usersWorkCenter', 'effectiveWorkPresenceVSTele', 'telecommutingForecastAndTrend')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE [Name] = 'telecommutingForecastAndTrend')
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (0,	'telecommutingForecastAndTrend','',		'C',	1,	'20211222',	'',	0,	'20211201',	'20220131',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"}],"columns":[{"uniqueName":"TelecommutingExpected","filter":{"members":["telecommutingexpected.[x]"]}},{"uniqueName":"DayOfWeek"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Hours\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-12-22T12:24:31.527Z"}','','','',				'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_Schedule(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter)',	'Calendar',	'Calendar.Analytics')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'hoju')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('hoju'
           ,'causesByEmployeeDate')
GO


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 and Class = 'hoju')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'hoju')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 7 and Class = 'hoju')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (7
           ,'hoju')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE Class = 'confic')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('confic'
           ,'conceptsColumns')
GO


IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 and Class = 'confic')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'confic')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 and Class = 'hoju')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'confic')
GO

DELETE FROM [dbo].[GeniusViews] WHERE name = 'effectiveWorkPresenceVSTele' and idpassport = 0
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (0,	'effectiveWorkPresenceVSTele','',		'C',	1,	'20211222',	'',	0,	'20211222',	'20211222',	'{"slice":{"rows":[{"uniqueName":"WorkCenter"},{"uniqueName":"Mes"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Office_ToHours","aggregation":"sum"},{"uniqueName":"Telecommute_ToHours","aggregation":"sum"},{"uniqueName":"%","formula":"sum(\"Telecommute_ToHours\") / (sum(\"Office_ToHours\") + sum(\"Telecommute_ToHours\") )","format":"-5c5v3gxgfjf00"}],"expands":{"expandAll":true},"flatOrder":["WorkCenter","Date_ToDateString","Office_ToHours","Telecommute_ToHours"]},"options":{"grid":{"type":"classic","showTotals":"off"},"chart":{"type":"stacked_column","activeMeasure":{"uniqueName":"Fórmula #1","aggregation":"none"}}},"formats":[{"name":"-5c5v3gxgfjf00","decimalPlaces":2,"isPercent":true}],"creationDate":"2021-10-06T11:12:36.972Z"}','','','','{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}','Genius_EfectiveWork(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter)',	'Calendar',	'Calendar.Analytics')
GO

DELETE FROM [dbo].[GeniusViews] WHERE Name = 'causesConceptsAndPunches' and IdPassport = -1

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusViews] WHERE Name = 'causesConceptsAndPunches' and IdPassport = -1)
INSERT INTO [dbo].[GeniusViews]
           ([IdPassport]
           ,[Name]
           ,[Description]
           ,[DS]
           ,[TypeView]
           ,[CreatedOn]
           ,[Employees]
           ,[DateFilterType]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[Concepts]
           ,[UserFields]
           ,[BusinessCenters]
           ,[CustomFields]
           ,[DSFunction]
           ,[Feature]
           ,[RequieredFeature])
     VALUES
           (-1,	'causesConceptsAndPunches', '','S',	1,	'20220111',''	,	0,	'20220111',	'20220111',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"IncidenceName"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Punches"},{"uniqueName":"ConceptName"},{"uniqueName":"ValueHHMM_ToHours"}],"columns":[{"uniqueName":"CauseName"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"CauseValue\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2022-01-20T09:37:50.150Z"}'		,'','','',		'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_ConceptsAndScheduleAndIncidencesAndPunches(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros))',	'Calendar',	'Calendar.Analytics')

GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportViews] WHERE CLASS = 'jusafi')
INSERT INTO [dbo].[GeniusCustomReportViews]
           ([Class]
           ,[NameViewResult])
     VALUES
           ('jusafi'
           ,'causesConceptsAndPunches')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 1 AND Class = 'jusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (1
           ,'jusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 2 AND Class = 'jusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (2
           ,'jusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[GeniusCustomReportCombs] WHERE IdCheck = 6 AND Class = 'jusafi')
INSERT INTO [dbo].[GeniusCustomReportCombs]
           ([IdCheck]
           ,[Class])
     VALUES
           (6
           ,'jusafi')
GO

IF NOT EXISTS (SELECT * FROM [dbo].[sysroGUI_Actions] WHERE IDPath = 'Convert2System' and IDGUIPath = 'Portal\General\Genius')
INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath]
           ,[IDGUIPath]
           ,[LanguageTag]
           ,[RequieredFeatures]
           ,[RequieredFunctionalities]
           ,[AfterFunction]
           ,[CssClass]
           ,[Section]
           ,[ElementIndex]
           ,[AppearsOnPopup]
           ,[Edition])
     VALUES
           ('Convert2System',	'Portal\General\Genius',	'tbCovert',	NULL,	NULL,	'convert2SystemCurrentGenius()',	'btnConvert',	0,	6,	0,	NULL)
GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"IDContract"},{"uniqueName":"UserField2"},{"uniqueName":"UserField1"},{"uniqueName":"UserField3"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField4","IDContract","UserField2","UserField1","UserField3","Age"]},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2021-12-16T15:19:03.964Z"}'					 ,
   UserFields = 'Categoría profesional,Género,Grupo de cotización,NIF'
 WHERE Name = 'usersContract' and IdPassport = 0
GO

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField7"},{"uniqueName":"UserField4"},{"uniqueName":"UserField8"},{"uniqueName":"UserField1"},{"uniqueName":"UserField5"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField7","UserField6","UserField4","UserField8","UserField1","UserField5","Age"]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2022-01-20T11:40:24.414Z"}'					 ,
   UserFields = 'Categoría profesional,Fecha de nacimiento,Fecha Nacimiento,Género,Grupo de cotización,Nacionalidad,NIF,Puesto'
 WHERE Name = 'usersWorkCenter' and IdPassport = 0
GO



UPDATE sysroParameters SET Data='526' WHERE ID='DBVersion'
GO