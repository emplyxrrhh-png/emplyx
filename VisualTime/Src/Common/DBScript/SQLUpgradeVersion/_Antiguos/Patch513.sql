CREATE NONCLUSTERED INDEX [IX_DocAlerts_Days]
ON [dbo].[Documents] ([Status])
INCLUDE ([IdDocumentTemplate],[IdEmployee],[IdDaysAbsence],[StatusLevel],[IdRequest])
GO

CREATE NONCLUSTERED INDEX [IX_DocAlerts_Hours]
ON [dbo].[Documents] ([Status])
INCLUDE ([IdDocumentTemplate],[IdEmployee],[IdHoursAbsence],[StatusLevel],[IdRequest])
GO

 ALTER PROCEDURE [dbo].[Analytics_Requests_Schedule]  
        @initialDate smalldatetime,        
        @endDate smalldatetime,        
        @idpassport int,        
        @employeeFilter nvarchar(max),        
        @userFieldsFilter nvarchar(max),      
 	   @requestTypesFilter nvarchar(max)        
   AS        
        DECLARE @employeeIDs Table(idEmployee int)        
     DECLARE @requestTypeFilter Table(idRequest int)        
 	  DECLARE @SecurityMode int
 	  SELECT @SecurityMode = convert(numeric(3),isnull(value,'1')) from sysroLiveAdvancedParameters where ParameterName = 'SecurityMode'
        insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @employeeFilter,@initialDate,@endDate;        
     insert into @requestTypeFilter select * from dbo.SplitToInt(@requestTypesFilter,',');         
        SELECT     
 	   
 	   dbo.requests.ID AS KeyView, isnull(case when aux.NumeroDias > 0 then aux.NumeroDias else DATEDIFF(day,dbo.Requests.Date1, dbo.Requests.Date2) + 1 end,0) as NumeroDias, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.sysroEmployeeGroups.IDEmployee,         
                              dbo.Employees.Name AS EmployeeName, dbo.EmployeeContracts.IDContract, dbo.Requests.RequestDate as 'Date',         
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
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.Date1, 120),'') as 'BeginDateRequest',        
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.FromTime, 8),'') as 'BeginHourRequest',        
          isnull(CONVERT(NVARCHAR(4000), isnull(dbo.Requests.Date2,dbo.Requests.Date1), 120),'') as 'EndDateRequest',        
          isnull(CONVERT(NVARCHAR(4000), dbo.Requests.ToTime, 8),'') as 'EndHourRequest',        
          ISNULL((SELECT Name from dbo.Causes WHERE ID = dbo.Requests.IDCause),'') as 'CauseNameRequest',        
          ISNULL((SELECT Name from dbo.Shifts WHERE ID = dbo.Requests.IDShift),'') as 'ShiftNameRequest',        
          CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')) as CommentsRequest,        
                              CONVERT(nvarchar(8), dbo.Requests.RequestDate, 108) AS Time, DAY(dbo.Requests.RequestDate) AS Day,         
                              MONTH(dbo.Requests.RequestDate) AS Month, YEAR(dbo.Requests.RequestDate) AS Año, (DATEPART(dw, dbo.Requests.RequestDate) + @@DATEFIRST - 1 - 1)         
                              % 7 + 1 AS DayOfWeek, 1 AS Value, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate,         
                              dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,         
               dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.EndDate AS EndContract, @initialDate AS BeginPeriod, @endDate AS EndPeriod,        
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
             dbo.GetEmployeeUserFieldValueMin(sysroEmployeeGroups.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@userFieldsFilter,10,','),dbo.Requests.RequestDate) As UserField10        
        FROM         dbo.sysroEmployeeGroups with (nolock) INNER JOIN        
                              dbo.requests with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.requests.IDEmployee AND dbo.requests.RequestDate >= dbo.sysroEmployeeGroups.BeginDate AND         
                              dbo.requests.RequestDate <= dbo.sysroEmployeeGroups.EndDate INNER JOIN        
                              dbo.EmployeeContracts with (nolock) ON dbo.requests.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.requests.RequestDate >= dbo.EmployeeContracts.BeginDate AND         
                              dbo.requests.RequestDate <= dbo.EmployeeContracts.EndDate LEFT OUTER JOIN        
                              dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
 							 left join (SELECT IDRequest, COUNT(*) NumeroDias FROM sysroRequestDays GROUP BY IDRequest) aux on aux.IDRequest = requests.id
        WHERE dbo.GetRequestPassportPermission(@idpassport, Requests.ID) > 1        
          AND dbo.sysroEmployeeGroups.IDEmployee in( select idEmployee from @employeeIDs) and dbo.requests.RequestDate between @initialDate and @endDate        
          and dbo.Requests.RequestType in (select idRequest from @requestTypeFilter)      
        GROUP BY dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.IDEmployee, MONTH(dbo.requests.RequestDate), YEAR(dbo.requests.RequestDate), dbo.requests.ID,         
                              dbo.sysroEmployeeGroups.FullGroupName, DAY(dbo.requests.RequestDate), dbo.requests.RequestDate, CONVERT(nvarchar(8), dbo.requests.RequestDate, 108),         
                              dbo.requests.RequestDate,  dbo.EmployeeContracts.IDContract, dbo.Employees.Name,         
                              (DATEPART(dw, dbo.requests.RequestDate) + @@DATEFIRST - 1 - 1) % 7 + 1, dbo.requests.RequestType, dbo.requests.Status, dbo.requests.Date1, dbo.requests.FromTime, dbo.requests.Date2, dbo.requests.ToTime, dbo.requests.IDCause, dbo.requests.IDShift,CONVERT(NVARCHAR(4000),isnull(dbo.Requests.Comments, '')), dbo.sysroEmployeeGroups.Path,         
                              dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate, dbo.sysroEmployeeGroups.IDGroup,         
          dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate        , aux.NumeroDias
        HAVING      (dbo.Requests.RequestType IN(2,3,4,5,6,7,8,9,11,14,15))
GO


ALTER TABLE dbo.GeniusViews ADD
	DSFunction nvarchar(MAX) NULL,
	Feature nvarchar(50) NULL
GO

DELETE from dbo.GeniusExecutions
GO

DELETE from dbo.GeniusViews
GO

DELETE from dbo.sysroLiveTasks where Parameters like '%<Item key="APIVersion" type="2">2</Item>%'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[GeniusViews] WHERE [Name] = 'scheduleByEmployeeDate')
    INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
	    VALUES (0, N'scheduleByEmployeeDate', N'', N'S', 2, CAST(N'2021-04-08T08:06:02.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Hours\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:51:52.161Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
ELSE
    UPDATE [dbo].[GeniusViews] SET [CubeLayout] =N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Hours\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:51:52.161Z"}'
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
	VALUES (0, N'requestsByEmployee', N'', N'S', 5, CAST(N'2021-04-08T08:31:34.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"},{"uniqueName":"TypeDesc"},{"uniqueName":"StatusDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:50:37.290Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
GO
INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
	VALUES (0, N'causesByEmployeeDate', N'', N'S', 3, CAST(N'2021-04-08T08:09:06.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"},{"uniqueName":"IncidenceName"},{"uniqueName":"ZoneTime"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-19T15:45:54.171Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
GO
INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
	VALUES (0, N'conceptsByEmployeeDate', N'', N'S', 1, CAST(N'2021-04-07T11:51:32.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ConceptName"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-19T15:44:09.661Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
GO
INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature]) 
	VALUES (0, N'users', N'', N'S', 6, CAST(N'2021-04-08T09:30:31.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"MethodDesc"},{"uniqueName":"Credential"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T08:07:48.061Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
GO
INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields], [DSFunction], [Feature])
	VALUES (0, N'punches', N'', N'S', 4, CAST(N'2021-04-08T08:23:40.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-19T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year"},{"uniqueName":"Date.Month"},{"uniqueName":"Date.Day"},{"uniqueName":"Time"},{"uniqueName":"TerminalName"},{"uniqueName":"PDDesc"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"sorting":{"column":{"type":"asc","tuple":[],"measure":{"uniqueName":"Value","aggregation":"sum"}}},"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-19T15:46:46.872Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}', N'', N'')
GO

 UPDATE sysroParameters SET Data='513' WHERE ID='DBVersion'
 GO

 