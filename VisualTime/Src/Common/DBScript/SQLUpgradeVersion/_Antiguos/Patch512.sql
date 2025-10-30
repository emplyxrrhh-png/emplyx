INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Justificaciones por empleado y fecha', N'', N'S', 3, CAST(N'2021-04-08T08:09:06.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"},{"uniqueName":"IncidenceName"},{"uniqueName":"ZoneTime"},{"uniqueName":"CauseName"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}]},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"columns"}},"creationDate":"2021-04-13T07:33:03.235Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Saldos agrupados por empleado', N'', N'S', 1, CAST(N'2021-04-07T11:51:32.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ConceptName"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T07:28:36.036Z"}', N'', N'', N'', N'{"IncludeZeros":true,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Planificación por empleado y fecha', N'', N'S', 2, CAST(N'2021-04-08T08:06:02.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"ShiftName"}],"measures":[{"uniqueName":"Hours","aggregation":"sum"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T07:42:39.798Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Solicitudes por empleado', N'', N'S', 5, CAST(N'2021-04-08T08:31:34.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"TypeDesc"},{"uniqueName":"StatusDesc"},{"uniqueName":"PendingPassport"},{"uniqueName":"ApproveRefusePassport"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T08:08:41.133Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Biometria', N'', N'S', 6, CAST(N'2021-04-08T09:30:31.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"MethodDesc"},{"uniqueName":"Credential"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}]},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T08:07:48.061Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

INSERT [dbo].[GeniusViews] ([IdPassport], [Name], [Description], [DS], [TypeView], [CreatedOn], [Employees], [DateFilterType], [DateInf], [DateSup], [CubeLayout], [Concepts], [UserFields], [BusinessCenters], [CustomFields]) 
	VALUES (0, N'Fichajes por empleado', N'', N'S', 4, CAST(N'2021-04-08T08:23:40.000' AS DateTime), N'', N'8', CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-04-13T23:59:59.000' AS DateTime), N'{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"GroupName"},{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date.Year","caption":"Date.Año"},{"uniqueName":"Date.Month","caption":"Date.Mes"},{"uniqueName":"Date.Day","caption":"Date.Día"},{"uniqueName":"Time"}],"columns":[{"uniqueName":"[Measures]"},{"uniqueName":"TerminalName"},{"uniqueName":"PDDesc"}],"measures":[{"uniqueName":"Value","aggregation":"sum"}],"sorting":{"column":{"type":"asc","tuple":["terminalname.[portal]"],"measure":{"uniqueName":"Value","aggregation":"sum"}}}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-04-13T07:57:31.323Z"}', N'', N'', N'', N'{"IncludeZeros":false,"IncludeZeroBusinessCenter":false,"LanguageKey":"ESP","RequestTypes":"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15"}')
GO

--Gestión del menú para Gestión Documental
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI] WHERE [URL] = '/DocumentaryManagement')

    insert into [dbo].sysroGUI(IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
         values ('Portal\General\DocumentaryManagement','DocumentaryManagement','/DocumentaryManagement','Documents.png',NULL,'SaaSEnabled',NULL,NULL,106,NULL,'U:Documents.DocumentsDefinition=Read')
ELSE
update [dbo].sysroGUI SET [Parameters] = 'SaaSEnabled', IDPath = 'Portal\General\DocumentaryManagement', LanguageReference = 'DocumentaryManagement'  WHERE (IDPath = 'Portal\GeneralManagement\DocumentaryManagement' OR IDPath = 'Portal\General\DocumentaryManagement')
GO


DELETE [dbo].sysroGUI_Actions WHERE IDGUIPath = 'Portal\GeneralManagement\DocumentaryManagement'
GO
DELETE [dbo].sysroGUI_Actions WHERE IDGUIPath = 'Portal\General\DocumentaryManagement'
GO
INSERT INTO [dbo].sysroGUI_Actions(IDPath,IDGUIPath,LanguageTag,RequieredFeatures,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex,AppearsOnPopup)
values
		('MaxMinimize','Portal\General\DocumentaryManagement','tbMaximize',NULL,NULL,'MaxMinimize()','btnMaximize2',0,1,0),
		('New','Portal\General\DocumentaryManagement','tbAddNewDocumentTemplate',NULL,NULL,'addNewDocumentTemplate()','btnAddNew2',0,2,0),
		('Edit','Portal\General\DocumentaryManagement','tbEditDocumentTemplate',NULL,NULL,'editCurrentDocumentTemplate()','btnEdit2',0,3,0),
		('Delete','Portal\General\DocumentaryManagement','tbDeleteDocumentTemplate',NULL,NULL,'deleteCurrentDocumentTemplate()','btnTbDel2',0,4,0)
GO

alter table dbo.sysropassports  ADD CurrentLastLoginDate datetime DEFAULT null
GO

update dbo.sysroGUI set Priority = 1401 where IDPath ='Portal\Access'
GO

update dbo.sysroGUI set Priority = 1402 where IDPath ='Portal\AccessManagement'
GO

update dbo.sysroGUI set Priority = 1501 where IDPath ='Portal\Task'
GO

update dbo.sysroGUI set Priority = 1601 where IDPath ='Portal\CostControl'
GO

insert into dbo.sysroGUI(IDPath,LanguageReference,URL,IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity,RequiredFunctionalities)
Values('Portal\Reports','Reports',NULL,'ReportScheduler.png',NULL,NULL,NULL,NULL,1301,NULL,NULL)
GO

update dbo.sysroGUI set IDPath = 'Portal\Reports\AdvReport',Priority=101 where IDPath ='Portal\General\AdvReport'
GO
update dbo.sysroGUI set IDPath = 'Portal\Reports\Genius',Priority=102 where IDPath ='Portal\General\Genius'
GO
update dbo.sysroGUI set IDPath = 'Portal\Reports\TasksQueue',Priority=103 where IDPath ='Portal\General\TasksQueue'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroParameters] WHERE [ID] = 'RuntimeID')
	insert into [dbo].sysroParameters (ID,Data) values ('RuntimeID','Z7W0-XHJG5L-1X3N0C-4U494G-3L5V1H-1G200U-3P025M-2R2Y3X-0X2H6E-0M440P-3W')
GO

 UPDATE sysroParameters SET Data='512' WHERE ID='DBVersion'
 GO

 