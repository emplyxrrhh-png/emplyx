--Nuevos modos de terminales para la nueva pantalla
update dbo.sysroReaderTemplates Set InteractionAction = 'E,S,X' where InteractionMode = 'Fast' or InteractionMode = 'Interactive'
GO
update dbo.sysroReaderTemplates Set InteractionAction = 'X' where InteractionMode = 'Interactive'
GO
update dbo.sysroReaderTemplates Set InteractionAction = 'E,S' where InteractionMode = 'Interactive' and Type = 'LivePortal'
GO
update dbo.sysroReaderTemplates Set InteractionMode = 'Fast' where Type = 'LivePortal'
GO
update dbo.sysroReaderTemplates set InteractionAction = 'X' where Type = 'LivePortal' and ScopeMode like '%TA%'
GO
delete from dbo.sysroReaderTemplates where ScopeMode = 'TA' and Type='LivePortal'
GO

--Adecuación de lectores de terminales mx7, mx8 y LivePortal
update dbo.terminalreaders set interactionaction = 'X' 
where interactionaction is NULL and InteractionMode = 'Interactive' and idterminal in (select id from dbo.terminals where type in ('MX8','MX7'))
GO

update dbo.terminalreaders set mode = 'TAEIP', interactionaction = 'ES', InteractionMode = 'Fast' 
where interactionaction = 'X' and InteractionMode = 'Interactive' and idterminal in (select id from dbo.terminals where type in ('LivePortal'))
GO

update dbo.terminalreaders set interactionaction = 'X', InteractionMode = 'Interactive' 
where mode = 'TA' and idterminal in (select id from dbo.terminals where type in ('MX8','MX7'))
GO

delete dbo.sysroreadertemplates where TYPE in ('MX8','MX7')
GO

INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 1, N'ACC', N'0', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 2, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 3, N'ACCTA', N'0', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 4, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 7, N'TA', N'1', N'Interactive', N'X', N'ServerLocal,Server', N'1,0', N'1,0', N'1,0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 9, N'TATSK', N'1', N'Interactive', N'X', N'ServerLocal,Server', N'1,0', N'1,0', N'1,0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 11, N'TSK', N'1', N'Interactive', N'X', N'ServerLocal,Server', N'1,0', N'0', N'1,0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 1, 14, N'DIN', N'1', N'Interactive', N'X', N'Server', N'1,0', N'0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 2, 14, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 2, 15, N'ACC', N'0', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 2, 16, N'ACC', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 2, 17, N'ACCTA', N'0', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx7', 2, 18, N'ACCTA', N'1', N'Blind', N'X', N'Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 42, N'TA', N'1', N'Fast', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 43, N'TA', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 44, N'TATSK', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 45, N'TSK', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 49, N'ACC', N'1', N'Blind', N'X', N'LocalServer,ServerLocal,Server,Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 51, N'ACCTA', N'1', N'Blind', N'X', N'LocalServer,ServerLocal,Server,Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 52, N'DIN', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 54, N'TAEIP', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 55, N'ACCTAEIP', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 56, N'ACCEIP', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'0', N'1,0', N'0', N'1,0', N'1,0', NULL, 1, N'0', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 57, N'TATSKEIP', N'1', N'Interactive', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 58, N'TAEIP', N'1', N'Fast', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 59, N'TATSK', N'1', N'Fast', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 1, 60, N'TATSKEIP', N'1', N'Fast', N'X', N'LocalServer,ServerLocal,Server,Local', N'1,0', N'1,0', N'0', N'0', N'0', NULL, 1, N'1', N'')
GO
INSERT [dbo].[sysroReaderTemplates] ([Type], [IDReader], [ID], [ScopeMode], [UseDispKey], [InteractionMode], [InteractionAction], [ValidationMode], [EmployeesLimit], [OHP], [CustomButtons], [Output], [InvalidOutput], [Direction], [AllowAccessPeriods], [SupportedSirens], [RequiredFeatures]) VALUES (N'mx8', 2, 46, N'', NULL, NULL, NULL, NULL, N'0', NULL, NULL, NULL, NULL, NULL, 1, N'1', N'')
GO

-- Modificación terminales para nuevo informe emergencias
ALTER TABLE dbo.TerminalReaders ADD
	IDZoneOut int NULL,
	PictureXOut numeric(8, 3) NULL,
	PictureYOut numeric(8, 3) NULL,
	IDCameraOut int NULL
GO

ALTER TABLE dbo.Groups ADD
	IDZoneWorkingTime int NULL,
	IDZoneNonWorkingTime int NULL
GO

-- Reorganización menú principal
DELETE FROM dbo.sysroGUI WHERE (IDPath LIKE 'Portal%')
GO

INSERT INTO dbo.sysroGUI VALUES('Portal',NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,NULL,NULL)
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Access','Access',NULL,'Access.png',NULL,NULL,'Forms\Access',NULL,1004,NULL,'U:Access=Read')
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Access\AccessGroups','GUI.AccessGroups','Access/AccessGroups.aspx','AccessGroups.png',NULL,NULL,'Forms\Access',NULL,103,NULL,'U:Access.Groups=Read') 
GO 
INSERT INTO dbo.sysroGUI VALUES('Portal\Access\AccessPeriods','GUI.AccessPeriods','Access/AccessPeriods.aspx','AccessPeriods.png',NULL,NULL,'Forms\Access',NULL,104,NULL,'U:Access.Periods=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Access\AccessStatus','GUI.AccessStatus','Access/AccessStatus.aspx','AccessStatus.png',NULL,NULL,'Forms\Access',NULL,101,NULL,'U:Access.Zones=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Access\Analytics','AccessAnalytics','Access/Analytics.aspx','AccessAnalytics.png',NULL,NULL,'Forms\Access',NULL,102,NULL,'U:Access.Analytics=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration','Configuration',NULL,'Administracion.ico',NULL,NULL,NULL,NULL,1010,'NWR','U:Administration.Options=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\AccessZones','GUI.AccessZones','Access/AccessZones.aspx','AccessZones.png',NULL,NULL,NULL,NULL,103,NULL,'U:Access.Zones=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\Cameras','Cameras','Cameras/Cameras.aspx','Cameras.png',NULL,NULL,'Version\Live',NULL,104,'NWR','U:Administration.Cameras.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\Notifications','Notifications','Notifications/Notifications.aspx','Notificaciones.png',NULL,NULL,'Process\Notifier',NULL,105,'NWR','U:Administration.Notifications.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Options.png',NULL,NULL,'Forms\Options',NULL,101,'NWR','U:Administration.Options.General=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\ReportScheduler','ReportScheduler','ReportScheduler/ReportScheduler.aspx','ReportScheduler.png',NULL,NULL,'Process\ReportServer',NULL,106,'NWR','U:Administration.ReportScheduler.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Configuration\Terminal','Terminals','Terminals/Terminals.aspx','TerminalesRT.png',NULL,NULL,'Forms\Terminals',NULL,102,'NWR','U:Terminals=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\DiningRoom','DiningRoom',NULL,'DiningRoom.png',NULL,NULL,'Forms\DiningRoom',NULL,1008,NULL,'U:DiningRoom=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\DiningRoom\DiningRoomTurn','DiningRoom','DiningRoom/DiningRoom.aspx','DiningRoom.png',NULL,NULL,'Forms\DiningRoom',NULL,101,NULL,'U:DiningRoom.Turns=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\General','General',NULL,'Maintenances.ico',NULL,NULL,'Forms\Employees',NULL,1001,NULL,NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\General\DataLink','DataLink','DataLink/DataLink.aspx','DataLink.png',NULL,NULL,'Forms\DataLink',NULL,102,NULL,'U:Employees=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\General\Employees','GUI.General.Employees','Employees/Employees.aspx','Employees.png',NULL,NULL,'Forms\Employees',NULL,101,'NWR','U:Employees=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\General\Requests','Supervisor.Requests','Requests/Requests.aspx','Requests.png',NULL,NULL,'Feature\LivePortal',NULL,103,NULL,'U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read OR U:Tasks.Requests.Forgotten=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,1006,NULL,NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\LabAgree\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,101,NULL,'U:LabAgree.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\LabAgree\LabAgreeRules','LabAgreeRules','LabAgree/LabAgreeRules.aspx','LabAgreeRules.png',NULL,NULL,'Feature\ConcertRules',NULL,102,NULL,'U:LabAgree.AccrualRules=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\LabAgree\StartupValues','StartupValues','LabAgree/StartupValues.aspx','StartupValues.png',NULL,NULL,'Feature\ConcertRules',NULL,103,NULL,'U:LabAgree.StartupValues=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\OHP','OHP',NULL,'OHP.png',NULL,NULL,'Feature\OHP',NULL,1007,NULL,NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\OHP\Activities','Activities','Activities/Activities.aspx','Activity.png',NULL,NULL,'Forms\Activities',NULL,101,NULL,'U:Activities.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Security','Security',NULL,'Security.png',NULL,NULL,NULL,NULL,1009,NULL,NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Security\Aministration','GUI.SaaSAdministration','Security/SaaSAdmin.aspx','AdminSaaS.png',NULL,'RoboticsEmployee','Forms\Passports',NULL,104,NULL,'U:Administration.Security=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Security\Audit','Audit','Audit/Audit.aspx','Audit.png',NULL,NULL,NULL,NULL,102,'NWR','U:Administration.Audit=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Security\Options','SecurityOptions','Security/SecurityOptions.aspx','SecurityOptions.png',NULL,NULL,'Forms\Passports',NULL,103,NULL,'U:Administration.SecurityOptions=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Security\Passports','GUI.Passports','Security/Passports.aspx','Passports.png',NULL,NULL,'Forms\Passports',NULL,101,NULL,'U:Administration.Security=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftControl','GUI.ShiftControl',NULL,'Presencia.ico',NULL,NULL,NULL,NULL,1002,'NWR',NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftControl\AbsencesStatus','GUI.Absences','Absences/AbsencesStatus.aspx','AbsencesStatus.png',NULL,NULL,'Feature\Absences',NULL,103,'NWR','U:Employees=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftControl\Analytics','GUI.CalendarAnalytics','Scheduler/AnalyticsScheduler.aspx','CalendarAnalytics.png',NULL,NULL,'Forms\Calendar',NULL,102,NULL,'U:Calendar=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftControl\Scheduler','GUI.Scheduler','Scheduler/Scheduler.aspx','Scheduler.png',NULL,NULL,'Forms\Calendar',NULL,101,'NWR','U:Calendar=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement','Gui.ShiftManagement',NULL,'Presencia.ico',NULL,NULL,NULL,NULL,1005,NULL,NULL) 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\AbsencesDocuments','GUI.AbsencesDocuments','Absences/DocumentsAbsences.aspx','DocumentsAbsences.png',NULL,NULL,'Feature\Absences',NULL,107,'NWR','U:Employees=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\Accruals','GUI.Accruals','Concepts/Concepts.aspx','Concepts.png',NULL,NULL,'Forms\Concepts',NULL,101,'NWR','U:Concepts.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\Assignments','Assignments','Assignments/Assignments.aspx','Assignment.png',NULL,NULL,'Feature\HRScheduling',NULL,104,NULL,'U:Assignments.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\Causes','GUI.Causes','Causes/Causes.aspx','Causes.png',NULL,NULL,'Forms\Causes',NULL,103,'NWR','U:Causes.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\KPI','KPI','Indicators/Indicators.aspx','Indicators.png',NULL,NULL,'Feature\KPIs',NULL,106,NULL,'U:KPI.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\Shifts','GUI.Shifts','Shifts/ShiftsExpress.aspx','Shifts.png',NULL,NULL,'Forms\ShiftsExpress',NULL,102,'NWR','U:Shifts.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\ShiftManagement\ShiftsPro','GUI.Shifts','Shifts/Shifts.aspx','Shifts.png',NULL,NULL,'Forms\Shifts',NULL,102,'NWR','U:Shifts.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Task','ProductiV',NULL,'Task.png',NULL,NULL,'Feature\Productiv',NULL,1003,NULL,'U:Tasks.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Task\Analytics','TaskAnalytics','Tasks/Analytics.aspx','TaskAnalytics.png',NULL,NULL,'Feature\Productiv',NULL,102,NULL,'U:Tasks.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Task\BusinessCenters','BusinessCenters','Tasks/BusinessCenters.aspx','BusinessCenters.png',NULL,NULL,'Feature\Productiv',NULL,104,NULL,'U:Tasks.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Task\Tasks','Task.Status','Tasks/Tasks.aspx','Task.png',NULL,NULL,'Feature\Productiv',NULL,101,NULL,'U:Tasks.Definition=Read') 
GO
INSERT INTO dbo.sysroGUI VALUES('Portal\Task\TaskTemplates','TaskTemplates','TaskTemplates/TaskTemplates.aspx','TaskTemplates.png',NULL,NULL,'Feature\Productiv',NULL,103,NULL,'U:Tasks.TemplatesDefinition=Read') 
GO


-- Notificaciones de terminales desconectados plus
ALTER TABLE dbo.Terminals ADD	LastStatusNotified nvarchar(50) NULL
GO

UPDATE dbo.Terminals  SET LastStatusNotified = LastStatus
GO

--sysrovwVisitsPresenceStatusPunches:
CREATE VIEW [dbo].[sysrovwVisitsPresenceStatusPunches]
AS
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
                             (SELECT TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader, 
							  (SELECT TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'IN' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.BeginTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
union
SELECT        vp.EmpVisitedId AS IDEmployee, MAX(vm.BeginTime) AS DateTime,
                             (SELECT TOP 1 sc.IDReader
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc
                               WHERE        vp.EmpVisitedId = sc.IDEmployee
                               ORDER BY sc.DateTime DESC) AS IDReader, 
							  (SELECT TOP 1 sc2.IDZone
                               FROM            dbo.sysrovwCurrentEmployeesPresenceStatusPunches sc2
                               WHERE        vp.EmpVisitedId = sc2.IDEmployee
                               ORDER BY sc2.DateTime DESC) AS IDZone, 'OUT' AS Status, vs.ID AS VisitorID, vs.Name AS Visitor
FROM            dbo.VisitPlan AS vp INNER JOIN
                         dbo.Visitors AS vs ON vs.ID = vp.VisitorId INNER JOIN
                         dbo.VisitMoves AS vm ON vm.VisitPlanId = vp.ID
WHERE        (vm.EndTime IS NOT NULL)
GROUP BY vp.EmpVisitedId, vs.Name, vp.PlannedById, vs.ID
GO

-- ReportScheduler
ALTER TABLE dbo.ReportsScheduler ADD EmergencyReportPriority BIT NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.ReportsScheduler ADD State tinyint NOT NULL DEFAULT(0)
GO
ALTER TABLE dbo.ReportsScheduler ADD LastExecution smalldatetime NULL
GO

-- [sysrovwCurrentEmployeesZoneStatus]
CREATE VIEW [dbo].[sysrovwCurrentEmployeesZoneStatus]
AS
SELECT        p.IDEmployee, em.Name AS EmployeeName, p.DateTime, p.IDTerminal AS IDReader, p.IDZone
FROM            dbo.Punches AS p INNER JOIN
                             (SELECT        dbo.Punches.IDEmployee, MAX(dbo.Punches.ID) AS idp
                               FROM            dbo.Punches INNER JOIN
                                                             (SELECT        IDEmployee, MAX(DateTime) AS dat
                                                               FROM            dbo.Punches AS Punches_1
                                                               GROUP BY IDEmployee) AS maxidat ON dbo.Punches.IDEmployee = maxidat.IDEmployee AND dbo.Punches.DateTime = maxidat.dat
                               WHERE        (NOT (dbo.Punches.IDZone IS NULL))
                               GROUP BY dbo.Punches.IDEmployee) AS maxPunch ON p.IDEmployee = maxPunch.IDEmployee AND p.ID = maxPunch.idp LEFT OUTER JOIN
                         dbo.Zones AS zo ON p.IDZone = zo.ID LEFT OUTER JOIN
                         dbo.Employees AS em ON p.IDEmployee = em.ID
GO

-- Seguridad portal del supervisor
CREATE TABLE [dbo].[TMPPermissions](
	[PassportID] [int] NOT NULL,
	[EmployeeID] [int] NULL,
	[PassportLevelOfAuthority] [int] NOT NULL,
	[EmployeePermission] [int] NOT NULL,
	[FeatureEmployeeID] [int] NOT NULL
) ON [PRIMARY]
GO


CREATE PROCEDURE [dbo].[ExecuteSupervisorPortalPermissions]
 AS
    DELETE FROM [dbo].[TMPPermissions]

	DECLARE @featureEmployeeID int
	DECLARE EmployeeFeatureCursor CURSOR
	FOR 
		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null

	OPEN EmployeeFeatureCursor
	FETCH NEXT FROM EmployeeFeatureCursor
	INTO @featureEmployeeID
	WHILE @@FETCH_STATUS = 0
	BEGIN	
		
		INSERT INTO [dbo].[TMPPermissions] (PassportID, EmployeeID,PassportLevelOfAuthority,EmployeePermission, FeatureEmployeeID) 
		select supPassports.ID, empPassports.IDEmployee,dbo.GetPassportLevelOfAuthority(supPassports.ID),
				dbo.WebLogin_GetPermissionOverEmployee(supPassports.ID,empPassports.IDEmployee,@featureEmployeeID,0,1,getdate()),@featureEmployeeID
				from sysroPassports supPassports, sysroPassports empPassports
				where supPassports.GroupType = 'U' and supPassports.Description NOT LIKE '@@ROBOTICS@@%' and
					  empPassports.IDEmployee is not null
					  and supPassports.ID NOT IN(SELECT IDPassport  FROM GetAllPassportParentsEmployeeType())
		
		FETCH NEXT FROM EmployeeFeatureCursor 
		INTO @featureEmployeeID
	END 
	CLOSE EmployeeFeatureCursor
	DEALLOCATE EmployeeFeatureCursor	
   
GO


CREATE PROCEDURE [dbo].[InsertPassportSupervisorPortalPermissions]
   	(
 		@IDPassport int
   	)

 AS
    DELETE FROM [dbo].[TMPPermissions] 	where PassportID=@IDPassport

	DECLARE @featureEmployeeID int
	DECLARE EmployeeFeatureCursor CURSOR
	FOR 
		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null

	OPEN EmployeeFeatureCursor
	FETCH NEXT FROM EmployeeFeatureCursor
	INTO @featureEmployeeID
	WHILE @@FETCH_STATUS = 0
	BEGIN	
		
		INSERT INTO [dbo].[TMPPermissions] (PassportID, EmployeeID,PassportLevelOfAuthority,EmployeePermission, FeatureEmployeeID) 
		select supPassports.ID, empPassports.IDEmployee,dbo.GetPassportLevelOfAuthority(supPassports.ID),
				dbo.WebLogin_GetPermissionOverEmployee(supPassports.ID,empPassports.IDEmployee,@featureEmployeeID,0,1,getdate()),@featureEmployeeID
				from sysroPassports supPassports, sysroPassports empPassports
				where supPassports.GroupType = 'U' and supPassports.Description NOT LIKE '@@ROBOTICS@@%' and
					  empPassports.IDEmployee is not null
					  and supPassports.ID NOT IN(SELECT IDPassport  FROM GetAllPassportParentsEmployeeType())
					  and supPassports.ID = @IDPassport
		
		
		FETCH NEXT FROM EmployeeFeatureCursor 
		INTO @featureEmployeeID
	END 
	CLOSE EmployeeFeatureCursor
	DEALLOCATE EmployeeFeatureCursor	
   
GO


CREATE PROCEDURE [dbo].[AlterPassportSupervisorPortalPermissions]
   	(
 		@IDParentPassport int
   	)

 AS
	BEGIN	
		DECLARE @idPassport int

		/* Obtenemos todos los pasaportes de tipo grupo que pueden supervisar y que estan dentro del arbol de grupos del pasaporte a actualizar */
		DECLARE db_cursor CURSOR FOR  
		SELECT ID from sysroPassports where GroupType = 'U' and ID NOT IN(SELECT IDPassport FROM dbo.GetAllPassportParentsEmployeeType()) and (ID in(SELECT ID FROM dbo.GetPassportChilds(@IDParentPassport)) or ID = @IDParentPassport)


		OPEN db_cursor  
		FETCH NEXT FROM db_cursor INTO @idPassport  
		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			DELETE FROM [dbo].[TMPPermissions] 	where PassportID=@IDPassport

			DECLARE @featureEmployeeID int
			DECLARE EmployeeFeatureCursor CURSOR
			FOR 
				SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null

			OPEN EmployeeFeatureCursor
			FETCH NEXT FROM EmployeeFeatureCursor
			INTO @featureEmployeeID
			WHILE @@FETCH_STATUS = 0
			BEGIN	
				
				INSERT INTO [dbo].[TMPPermissions] (PassportID, EmployeeID,PassportLevelOfAuthority,EmployeePermission, FeatureEmployeeID) 
				select supPassports.ID, empPassports.IDEmployee,dbo.GetPassportLevelOfAuthority(supPassports.ID),
						dbo.WebLogin_GetPermissionOverEmployee(supPassports.ID,empPassports.IDEmployee,@featureEmployeeID,0,1,getdate()),@featureEmployeeID
						from sysroPassports supPassports, sysroPassports empPassports
						where supPassports.GroupType = 'U' and supPassports.Description NOT LIKE '@@ROBOTICS@@%' and
							  empPassports.IDEmployee is not null
							  and supPassports.ID = @IDPassport
				
				
				FETCH NEXT FROM EmployeeFeatureCursor 
				INTO @featureEmployeeID
			END 
			CLOSE EmployeeFeatureCursor
			DEALLOCATE EmployeeFeatureCursor
		END  
		CLOSE db_cursor  
		DEALLOCATE db_cursor 		
	END
GO


 ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
   	(
 		@IDAction int,
   		@IDObject int
   	)
   AS
   /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
	BEGIN
		IF @IDAction = -1
		BEGIN
			exec dbo.GenerateAllRequestPassportPermission
			exec dbo.ExecuteSupervisorPortalPermissions
		END
		IF @IDAction = 0
		BEGIN
			exec dbo.InsertPassportRequestsPermission @IDObject
			exec dbo.InsertPassportSupervisorPortalPermissions @IDObject
		END
		IF @IDAction = 1
		BEGIN
			exec dbo.AlterPassportRequestsPermission @IDObject 
			exec dbo.AlterPassportSupervisorPortalPermissions @IDObject
		END
		IF @IDAction = 2
		BEGIN
			exec dbo.InsertRequestPassportPermission @IDObject
		END
	END
GO


exec dbo.ExecuteSupervisorPortalPermissions
go


ALTER FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
(
	@idPassport int,
	@featureAlias nvarchar(50)
)
RETURNS @result table (ID int PRIMARY KEY)
AS
	BEGIN
		DECLARE @EmployeeID int
		DECLARE @SupervisorLevel int
		DECLARE @featureEmployeeID int
		DECLARE @featurePermission int
		DECLARE @tmpMaxFeatureWithPermissionByEmployee table(EmployeeID int, PassportLevelOfAuthority int)
		
		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias)
		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
		
		IF @featurePermission > 3
			BEGIN
				
				INSERT INTO @tmpMaxFeatureWithPermissionByEmployee(EmployeeID,PassportLevelOfAuthority)
					select EmployeeID, MAX(PassportLevelOfAuthority) from [dbo].[TMPPermissions]
						where EmployeePermission > 3 and  FeatureEmployeeID  = @featureEmployeeID  group by EmployeeID
				
				DECLARE PassportsCursor CURSOR
					FOR
						SELECT EmployeeID
						FROM  	
						(select sysroPassports.ID, tPerm.EmployeeID, tPerm.PassportLevelOfAuthority as SupervisorLevel 
							from sysroPassports 
								inner join (
									select distinct perm.* from [dbo].[TMPPermissions] perm inner join @tmpMaxFeatureWithPermissionByEmployee empPerm
									on perm.EmployeeID = empPerm.EmployeeID and empPerm.PassportLevelOfAuthority = perm.PassportLevelOfAuthority 
									where perm.FeatureEmployeeID = @featureEmployeeID
								) tPerm
							on sysroPassports.IDParentPassport = tPerm.PassportID and tPerm.EmployeePermission > 3 and tPerm.PassportLevelOfAuthority = @SupervisorLevel
							where  sysroPassports.ID = @idPassport  
						) pasPerm
						
				OPEN PassportsCursor
				FETCH NEXT FROM PassportsCursor
				INTO @EmployeeID
				WHILE @@FETCH_STATUS = 0
				BEGIN	
					INSERT INTO @result VALUES (@EmployeeID)
					
					FETCH NEXT FROM PassportsCursor 
					INTO @EmployeeID
				END 
				CLOSE PassportsCursor
				DEALLOCATE PassportsCursor	
			END
		RETURN
	END
go

ALTER PROCEDURE [dbo].[WebLogin_Passports_Update] 
 (@id int,
 @idParentPassport int,
 @groupType varchar(1),
 @name nvarchar(50),
 @description nvarchar(4000),
 @email nvarchar(100),
 @idUser int,
 @idEmployee int,
 @idLanguage int,
 @levelOfAuthority tinyint,
 @ConfData text,
 @AuthenticationMerge nvarchar(50),
 @StartDate smalldatetime,
 @ExpirationDate smalldatetime,
 @State smallint,
 @EnabledVTDesktop bit,
 @EnabledVTPortal bit,
 @EnabledVTSupervisor bit,
 @EnabledVTPortalApp bit,
 @EnabledVTSupervisorApp bit,
 @NeedValidationCode bit,
 @TimeStampValidationCode smalldatetime,
 @ValidationCode  nvarchar(100))
AS
 IF @groupType <> 'U' 
 	BEGIN
 		SET @levelOfAuthority = NULL
 	END
 UPDATE sysroPassports SET
 	IDParentPassport = @idParentPassport,
 	GroupType = @groupType,
 	Name = @name,
 	Description = @description,
 	Email = @email,
 	IDUser = @idUser,
 	IDEmployee = @idEmployee,
 	IDLanguage = @idLanguage,
 	LevelOfAuthority = @levelOfAuthority,
 	ConfData = @ConfData,
 	AuthenticationMerge = @AuthenticationMerge,
 	StartDate = @StartDate,
 	ExpirationDate = @ExpirationDate,
 	[State] = @State, 
	EnabledVTDesktop = @EnabledVTDesktop,
	EnabledVTPortal = @EnabledVTPortal,
	EnabledVTSupervisor = @EnabledVTSupervisor,
	EnabledVTPortalApp = @EnabledVTPortalApp,
	EnabledVTSupervisorApp = @EnabledVTSupervisorApp,
	NeedValidationCode = @NeedValidationCode,
	TimeStampValidationCode = @TimeStampValidationCode,
	ValidationCode  = @ValidationCode 
 	
 WHERE ID = @id
 
IF @groupType = 'U' 
  	BEGIN
		-- 1 / Actualizar los permisos del grupo y sus hijos
		IF NOT EXISTS ( SELECT ID FROM RequestPassportPermissionsPending WHERE IDAction = 1 AND IDObject = @id)
			insert into RequestPassportPermissionsPending Values(1, @id)
  	END
  	 
 RETURN
GO
	
-- Parametros avanzados de VisualTimeLive
CREATE TABLE dbo.sysroLiveAdvancedParameters
	(
	ParameterName nvarchar(100) NOT NULL,
	Value nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.sysroLiveAdvancedParameters ADD CONSTRAINT
	PK_sysroLiveAdvancedParameters PRIMARY KEY CLUSTERED 
	(
	ParameterName
	)  ON [PRIMARY]

GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='323' WHERE ID='DBVersion'
GO

