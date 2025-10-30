--EDITIONS
UPDATE sysrogui SET Edition = '' WHERE Edition = 'Starter'
GO
UPDATE ExportGuides SET Edition = '' WHERE Edition = 'Starter'
GO
UPDATE ImportGuides SET Edition = '' WHERE Edition = 'Starter'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Edition = 'Starter'
GO
UPDATE sysroGUI_Actions SET Edition = '' WHERE Edition = 'Starter'
GO

--NOTIFICATIONTYPES
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (11, N'Terminal Disconnected', NULL, 2, N'Terminals.Definition', N'', 0, 6, CAST(N'2023-03-06T13:40:00.000' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (13, N'Employee Absence on coverage', NULL, -1, N'Calendar.Scheduler', N'U', 1, 6, CAST(N'2023-02-13T11:17:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (14, N'Under Coverage', NULL, -1, N'Calendar.Scheduler', N'U', 1, 6, CAST(N'2023-02-13T11:17:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (16, N'End Period Employee', NULL, 510, N'Employees.UserFields', N'U', 0, 6, CAST(N'2023-03-06T13:40:00.000' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (17, N'End Period Enterprise', NULL, 500, N'Employees.UserFields', N'U', 0, 6, CAST(N'2023-03-06T13:40:00.000' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (22, N'IDCard not assigned to employee', NULL, 5, N'Employees.IdentifyMethods', N'U', 0, 6, CAST(N'2023-03-06T13:45:13.037' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (23, N'Task close to finish', NULL, 320, N'Tasks.Definition', N'', 0, 6, CAST(N'2023-03-06T18:48:11.617' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (24, N'Task close to start', NULL, 330, N'Tasks.Definition', N'', 0, 6, CAST(N'2023-03-06T17:20:13.573' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (25, N'Task exceeding planned time', NULL, 340, N'Tasks.Definition', N'', 0, 6, CAST(N'2023-03-06T14:54:20.690' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (26, N'Task exceeding finished date', NULL, 350, N'Tasks.Definition', N'', 0, 6, CAST(N'2023-03-06T17:20:18.350' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (28, N'Request Shift Change', NULL, 125, N'Calendar.Requests.ShiftChange', N'U', 1, 6, CAST(N'2023-03-06T15:31:13.413' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (29, N'Task exceeding Started date', NULL, 400, N'Tasks.Definition', N'', 0, 6, CAST(N'2023-03-06T19:30:10.843' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (30, N'Tasks with alerts', NULL, 5, N'Tasks.Definition', N'', 1, 6, CAST(N'2023-03-06T13:45:13.047' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (31, N'Kpi Limit OverTaken', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (41, N'Employee Present With Expired Docs', NULL, -1, N'Employees.UserFields.Information', N'U', 1, 6, CAST(N'2023-02-13T11:20:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (45, N'Visit Update', NULL, -1, N'Employees', N'U', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (54, N'Productive Unit Under Coverage', NULL, -30, N'Budgets', N'U', 1, 6, CAST(N'2023-02-13T11:45:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (57, N'InvalidPortalConsents', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (58, N'InvalidDesktopConsents', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (59, N'InvalidTerminalConsents', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (60, N'InvalidVisitsConsents', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (62, N'LabAgree Max Exceeded', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (63, N'LabAgree Min Reached', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (64, N'Tasks Request complete', NULL, -1, N'', N'', 1, 6, CAST(N'2023-02-13T17:15:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (69, N'Employee without mask', NULL, -1, N'Calendar.Punches', N'U', 0, 6, CAST(N'2023-02-13T11:17:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (71, N'Scheduler Analytic Executed', NULL, -1, N'', N'', 1, 0, CAST(N'2023-02-13T11:17:16.170' AS DateTime), 0)
GO
INSERT [dbo].[sysroNotificationTypes] ([ID], [Name], [Description], [Scheduler], [Feature], [FeatureType], [OnlySystem], [IDCategory], [NextExecution], [IsRunning]) VALUES (73, N'Employee with high temperature', NULL, 2, N'Calendar.Punches', N'U', 0, 6, CAST(N'2023-03-06T13:40:00.000' AS DateTime), 0)
GO

--NOTIFICATIONS
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (901, 13, N'Cubrir empleado ausente', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (902, 14, N'Cobertura planificada insuficiente', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MaxDays" type="8">3</Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1004, 22, N'Tarjeta no asignada a ningún empleado', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1009, 11, N'Terminal desconectado', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1011, 23, N'Tareas que finalizan hoy', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1012, 24, N'Tareas que empiezan hoy', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1013, 25, N'Tareas donde se ha excedido el tiempo inicial', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1014, 26, N'Tareas donde se ha sobrepasado la fecha de finalización prevista', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1016, 28, N'Solicitud de cambio de horario', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1500, 29, N'Tareas donde se ha sobrepasado la fecha de inicio prevista', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (1550, 30, N'Tareas con alertas', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 1, 0, 0, 0, NULL, NULL, 0, N'', NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (2001, 41, N'Aviso de empleado presente con documentación caducada', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 0, 0, 0, 1, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (4501, 45, N'Modificacion de visitas', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 0, 0, 0, 0, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (903, 54, N'Cobertura insuficiente en una unidad productiva', N'<?xml version="1.0"?><roCollection version="2.0"><Item key="MaxDays" type="8">3</Item></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 0, 0, 0, 1, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Notifications] ([ID], [IDType], [Name], [Condition], [Destination], [Schedule], [Activated], [CreatorID], [AllowPortal], [IDPassportDestination], [AllowMail], [ShowOnDesktop], [LastTaskDeleted], [AllowVTPortal], [InProgress], [GUID], [LastCheck]) VALUES (4900, 71, N'Analitica planificada ejecutada', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', N'<?xml version="1.0"?><roCollection version="2.0"></roCollection>', NULL, 1, N'SYSTEM', 0, 0, 1, 0, NULL, NULL, 0, NULL, NULL)
GO

UPDATE [dbo].[sysroNotificationTypes] SET RequiresPermissionOverEmployees = 0
GO
UPDATE [dbo].[sysroNotificationTypes] SET RequiresPermissionOverEmployees = 1 WHERE ID IN (1,2,3,4,5,6,7,8,9,13,14,15,16,17,18,19,20,21,22,27,28,33,35,40,41,42,43,44,45,46,47,49,50,51,52,53,54,56,65,66,68,69,73,76,77,78,79,80,81,82,83,84,91)
GO

-- DOCUMENTS
UPDATE DocumentTemplates SET IsSystem = 0 WHERE ShortName <> 'r_c'
GO
