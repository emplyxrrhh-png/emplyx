IF (SELECT COUNT(*) FROM [sysroGUI_Actions] WHERE [IDPath] = 'AddPlanification') = 0
BEGIN
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
           ('AddPlanification',	'Portal\General\Genius',	'tbPlanification',	NULL,	NULL,	'addPlanificationCurrentGenius()',	'btnAddPlanification',	0,	7,	0,	NULL)

END
GO

CREATE TABLE [dbo].[Genius_Views_Scheduler](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdGeniusView] [int] NOT NULL,
	[Scheduler] [nvarchar](128) NOT NULL,
	[LastDateTimeExecuted] [smalldatetime] NULL,
	[LastDateTimeUpdated] [smalldatetime] NULL,
	[NextDateTimeExecuted] [smalldatetime] NULL,
	[name] [nvarchar](200) NOT NULL,
	[IdPassport] [int] NOT NULL,
	[ExecuteTask] [bit] NULL,
	[LastExecution] [smalldatetime] NULL,
	[SchedulerText] [nvarchar](200) NOT NULL,
	[State] [tinyint] NOT NULL,
	[PeriodType] [smallint] NULL,
	[ScheduleBeginDate] [datetime] NULL,
	[ScheduleEndDate] [datetime] NULL,
 CONSTRAINT [PK_Genius_Views_Scheduler] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Genius_Views_Scheduler]  WITH CHECK ADD  CONSTRAINT [FK_Genius_Views_Scheduler_GeniusViews] FOREIGN KEY([IdGeniusView])
REFERENCES [dbo].[GeniusViews] ([ID])
GO

ALTER TABLE [dbo].[Genius_Views_Scheduler] CHECK CONSTRAINT [FK_Genius_Views_Scheduler_GeniusViews]
GO

ALTER TABLE [dbo].[GeniusViews] ADD CheckedCheckboxes varchar(20)

GO

UPDATE dbo.GeniusViews set CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"PDDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:46:46.872Z"}' where name='punches' and IdPassport = -1

GO

CREATE PROCEDURE [dbo].[Genius_Concepts_Hours_Receipt]
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

         SELECT reqReg.idEmployee As IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, reqReg.idConcept AS IDConcept,   
  				dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, reqReg.employeeName AS EmployeeName,   
  				reqReg.conceptName AS ConceptName, 
				reqReg.dt AS Date, reqReg.dt AS Date_ToDateString, 
				sum(isnull(AccrualsPrice.Value,0)) AS Value, sum(isnull(AccrualsPrice.Value,0)) AS ValueHHMM_ToHours,COUNT(*) AS Count, 
				MONTH(reqReg.dt) AS Mes,   
  				YEAR(reqReg.dt) AS Año, (DATEPART(dw, reqReg.dt) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, 
				CASE WHEN DATEPART(iso_week, reqReg.dt) >= 52 AND MONTH(reqReg.dt) = 1 THEN (YEAR(reqReg.dt)-1) * 100 + DATEPART(iso_week, reqReg.dt)
				ELSE (YEAR(reqReg.dt)) * 100 + DATEPART(iso_week, reqReg.dt) END AS WeekOfYear, DATEPART(QUARTER, reqReg.dt) AS Quarter, dbo.sysroEmployeeGroups.Path,   
  				dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.BeginDate AS BeginDate_ToDateString, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.EndDate AS EndDate_ToDateString,
  				dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.BeginDate AS BeginContract_ToDateString, dbo.EmployeeContracts.EndDate AS EndContract, dbo.EmployeeContracts.EndDate AS EndContract_ToDateString, @initialDate AS BeginPeriod, @initialDate AS BeginPeriod_ToDateString, @pendDate AS EndPeriod, @pendDate AS EndPeriod_ToDateString,
CASE WHEN ISNULL(AccrualsPrice.Price,'') = '' THEN 0 ELSE CONVERT(Numeric(18,3),AccrualsPrice.Price) END AS Price, 
		dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),reqReg.dt) As UserField1,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),reqReg.dt) As UserField2,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),reqReg.dt) As UserField3,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),reqReg.dt) As UserField4,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),reqReg.dt) As UserField5,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),reqReg.dt) As UserField6,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),reqReg.dt) As UserField7,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),reqReg.dt) As UserField8,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),reqReg.dt) As UserField9,  
  				dbo.GetEmployeeUserFieldValueMin(reqReg.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),reqReg.dt) As UserField10
		FROM
		(select alldays.dt,emp.ID as idEmployee,con.ID as idConcept, emp.Name as employeeName, con.Name As conceptName from alldays,   
  				(select ID, Name from Employees with (nolock) inner join @employeeIDs selEmp on Employees.ID = selEmp.idEmployee) emp,   
  				(select ID, Name from Concepts with (nolock) inner join @conceptIDs selCon on Concepts.ID = selCon.idConcept)con  
  				where dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,emp.ID,2,0,0,alldays.dt) > 1  
  				) reqReg  
				INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.sysroEmployeeGroups.BeginDate and dbo.sysroEmployeeGroups.EndDate  
				INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.EmployeeContracts.IDEmployee = reqReg.idEmployee AND reqReg.dt between dbo.EmployeeContracts.BeginDate and dbo.EmployeeContracts.EndDate  				
		LEFT OUTER JOIN
		(
		SELECT DailyAccruals.IDEmployee, DailyAccruals.IDConcept, DailyAccruals.Date,
		CASE WHEN ISNULL(Concepts.FixedPay,0) <> 0 THEN
		convert(nvarchar(100),Concepts.PayValue)
		ELSE
		dbo.GetValueFromEmployeeUserFieldValues(DailyAccruals.IDEmployee, SUBSTRING(Concepts.UsedField,5,100), DailyAccruals.Date)
		END AS Price,
		DailyAccruals.Value, EmployeeContracts.IDContract, EmployeeContracts.BeginDate, EmployeeContracts.EndDate
		FROM DailyAccruals INNER JOIN Concepts ON DailyAccruals.IDConcept = Concepts.ID
		INNER JOIN EmployeeContracts ON
		DailyAccruals.IDEmployee = EmployeeContracts.IDEmployee AND
		DailyAccruals.Date >= EmployeeContracts.BeginDate AND
		DailyAccruals.Date <= EmployeeContracts.EndDate		
		) AS AccrualsPrice 
		ON
		reqReg.IDEmployee = AccrualsPrice.IDEmployee AND reqReg.IDConcept = AccrualsPrice.IDConcept AND AccrualsPrice.Date = reqReg.dt
		WHERE ISNULL(AccrualsPrice.Value,0) <> 0 AND ISNULL(AccrualsPrice.Price,'') <> ''
GROUP BY Price, 
		AccrualsPrice.IDContract, AccrualsPrice.BeginDate, AccrualsPrice.EndDate, reqReg.idEmployee, reqReg.dt
		, reqReg.idConcept,
		reqReg.employeeName, reqReg.conceptName, AccrualsPrice.IDEmployee, AccrualsPrice.IDConcept, --AccrualsPrice.Date,
		dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName,
		dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,
		dbo.sysroEmployeeGroups.EndDate, dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate
		ORDER BY sysroEmployeeGroups.FullGroupName, EmployeeName 
GO

IF (SELECT COUNT(*) FROM DBO.GeniusViews WHERE IdPassport = -1 AND NAME = 'hoursReceiptByContract') = 0
BEGIN
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
           (0,	'hoursReceiptByContract','','S',	1,	'20220315',	'',	0,	'20220315',	'20220315',	'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Price"}],"columns":[{"uniqueName":"ConceptName"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")"},{"uniqueName":"Importe","formula":"sum(\"Price\") * sum(\"Value\")","caption":"Importe"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"},"timePattern":"HH:mm"},"creationDate":"2022-03-15T12:45:24.218Z"}',	'',	'','',	'{"IncludeZeros":true,"IncludeZeroBusinessCenter":true,"LanguageKey":"ESP","RequestTypes":""}',	'Genius_Concepts_Hours_Receipt(@initialDate,@endDate,@idpassport,@employeeFilter,@conceptsFilter,@userFieldsFilter,@includeZeros))',	'Calendar',	'Calendar.Analytics')
END
GO

ALTER TABLE GeniusViews ADD IdParentShared int;
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuides] WHERE ID = 20010)
	INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile], [Edition])
	VALUES (20010, N'Comedores', N'', 17, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Employees.DataLink.Exports.AdvDinning', N'Employees', NULL, NULL, NULL, 2, N'Dinner', 0, 13, NULL, NULL, 0, N'', NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuidesTemplates] WHERE ID = 13)
	INSERT [dbo].[ExportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (13, 20010, 'dinner', 'tmplExportDinner.xlsx' , NULL, NULL, NULL)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='531' WHERE ID='DBVersion'
GO
