
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuides] WHERE ID = 20010)
	INSERT [dbo].[ExportGuides] ([ID], [Name], [ProfileMask], [ProfileType], [Mode], [ProfileName], [Destination], [ExportFileName], [ExportFileType], [Separator], [StartCalculDay], [StartExecutionHour], [LastLog], [NextExecution], [Field_1], [Field_2], [Field_3], [Field_4], [Field_5], [Field_6], [IntervalMinutes], [DisplayParameters], [Scheduler], [EmployeeFilter], [RequieredFunctionalities], [FeatureAliasID], [ExportFileNameTimeStampFormat], [WSParameters], [AutomaticDatePeriod], [Version], [Concept], [Active], [IDDefaultTemplate], [PostProcessScript], [Enabled], [ApplyLockDate], [DatasourceFile], [Edition])
	VALUES (20010, N'Comedores', N'', 17, 1, N'', N'', N'', 1, N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'1,1,0@1@1', N'', NULL, N'Employees.DataLink.Exports.AdvDinning', N'Employees', NULL, NULL, NULL, 2, N'Dinner', 0, 13, NULL, NULL, 0, N'', NULL)
GO

IF NOT EXISTS (SELECT * FROM [dbo].[ExportGuidesTemplates] WHERE ID = 13)
	INSERT [dbo].[ExportGuidesTemplates] ([ID], [IDParentGuide], [Name], [Profile], [Parameters], [PostProcessScript], [PreProcessScript])
	VALUES (13, 20010, 'dinner', 'tmplExportDinner.xlsx' , NULL, NULL, NULL)
GO

update sysroNotificationTypes set OnlySystem=1 where id=84
GO

UPDATE sysroParameters SET Data='534' WHERE ID='DBVersion'
GO