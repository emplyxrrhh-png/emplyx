DELETE FROM [dbo].[sysroGUI] WHERE [IDPath] LIKE 'Portal%' 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal',NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Access','Access',NULL,'Access.png',NULL,NULL,'Forms\Access',NULL,1301,NULL,'U:Access=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Access\AccessStatus','GUI.AccessStatus','Access/AccessStatus.aspx','AccessStatus.png',NULL,NULL,'Forms\Access',NULL,101,NULL,'U:Access.Zones=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Access\Analytics','AccessAnalytics','Access/Analytics.aspx','AccessAnalytics.png',NULL,NULL,'Forms\Access',NULL,102,NULL,'U:Access.Analytics=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Access\Events','Events','Access/Events.aspx','Events.png',NULL,NULL,'Feature\Events',NULL,103,NULL,'U:Events.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\AccessManagement','Gui.AccessManagement',NULL,'AccessManagement.png',NULL,NULL,NULL,NULL,1302,NULL,'U:Access=Read OR U:Activities.Definition=Read OR U:DiningRoom.Turns=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\AccessManagement\AccessGroups','GUI.AccessGroups','Access/AccessGroups.aspx','AccessGroups.png',NULL,NULL,'Forms\Access',NULL,101,NULL,'U:Access.Groups=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\AccessManagement\AccessPeriods','GUI.AccessPeriods','Access/AccessPeriods.aspx','AccessPeriods.png',NULL,NULL,'Forms\Access',NULL,102,NULL,'U:Access.Periods=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\AccessManagement\DiningRoomTurn','DiningRoom','DiningRoom/DiningRoom.aspx','DiningRoom.png',NULL,NULL,'Forms\DiningRoom',NULL,103,NULL,'U:DiningRoom.Turns=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration','Configuration',NULL,'Administracion.ico',NULL,NULL,NULL,NULL,2022,'NWR',NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration\EmergencyReport','EmergencyReport','Options/EmergencyReport.aspx','EmergencyPrint.png',NULL,NULL,'Forms\Options',NULL,105,NULL,'U:Administration.Options.General=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Options.png',NULL,NULL,'Forms\Options',NULL,103,'NWR','U:Administration.Options.General=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration\Routes','Routes','Options/RoutesManager.aspx','Routes.png',NULL,NULL,'Forms\Options',NULL,102,'NWR','U:Administration.Options.General=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration\SecurityOptions','SecurityOptions','Security/SecurityOptions.aspx','SecurityOptions.png',NULL,NULL,'Forms\Passports',NULL,104,NULL,'U:Administration.SecurityOptions=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Configuration\UserFields','UserFields','Options/FieldsManager.aspx','UserFields.png',NULL,NULL,'Forms\Options',NULL,101,'NWR','U:Administration.Options.General=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\CostControl','CostControl',NULL,'BusinessCenters.png',NULL,NULL,NULL,NULL,1501,NULL,'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\CostControl\Analytics','CostControlAnalytics','Scheduler/AnalyticsCostControl.aspx','CostAnalytics.png',NULL,NULL,'Feature\CostControl',NULL,102,NULL,'U:BusinessCenters.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\CostControl\BusinessCenters','BusinessCenters','Tasks/BusinessCenters.aspx','BusinessCenters.png',NULL,NULL,NULL,NULL,101,NULL,'U:Tasks.Definition=Read OR U:BusinessCenters.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\General','General',NULL,'Maintenances.ico',NULL,NULL,'Forms\Employees',NULL,1101,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\General\Alerts','Alerts','Alerts/Alerts.aspx','Alerts.png',NULL,NULL,NULL,NULL,104,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\General\DataLink','DataLink','DataLink/DataLink.aspx','DataLink.png',NULL,NULL,'Forms\DataLink',NULL,102,NULL,'U:Employees=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\General\Employees','GUI.General.Employees','Employees/Employees.aspx','Employees.png',NULL,NULL,'Forms\Employees',NULL,101,'NWR','U:Employees=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\General\Requests','Supervisor.Requests','Requests/Requests.aspx','Requests.png',NULL,NULL,NULL,NULL,103,NULL,'U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read OR U:Tasks.Requests.Fortten=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement','GeneralManagement',NULL,'GeneralManagement.png',NULL,NULL,NULL,NULL,1102,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\AccessZones','GUI.AccessZones','Access/AccessZones.aspx','AccessZones.png',NULL,NULL,NULL,NULL,102,NULL,'U:Access.Zones=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\Activities','Activities','Activities/Activities.aspx','Activity.png',NULL,NULL,'Forms\Activities',NULL,106,NULL,'U:Activities.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\Cameras','Cameras','Cameras/Cameras.aspx','Cameras.png',NULL,NULL,'Version\Live',NULL,103,'NWR','U:Administration.Cameras.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\Notifications','Notifications','Notifications/Notifications.aspx','Notificaciones.png',NULL,NULL,'Process\Notifier',NULL,104,'NWR','U:Administration.Notifications.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\ReportScheduler','ReportScheduler','ReportScheduler/ReportScheduler.aspx','ReportScheduler.png',NULL,NULL,'Process\ReportServer',NULL,105,'NWR','U:Administration.ReportScheduler.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\GeneralManagement\Terminal','Terminals','Terminals/Terminals.aspx','TerminalesRT.png',NULL,NULL,'Forms\Terminals',NULL,101,'NWR','U:Terminals=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,2002,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\LabAgree\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,101,NULL,'U:LabAgree.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\LabAgree\LabAgreeRules','LabAgreeRules','LabAgree/LabAgreeRules.aspx','LabAgreeRules.png',NULL,NULL,'Feature\ConcertRules',NULL,102,NULL,'U:LabAgree.AccrualRules=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\LabAgree\StartupValues','StartupValues','LabAgree/StartupValues.aspx','StartupValues.png',NULL,NULL,'Feature\ConcertRules',NULL,103,NULL,'U:LabAgree.StartupValues=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security','Security',NULL,'Security.png',NULL,NULL,NULL,NULL,2012,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security\Aministration','GUI.SaaSAdministration','Security/SaaSAdmin.aspx','AdminSaaS.png',NULL,'RoboticsEmployee','Forms\Passports',NULL,105,NULL,'U:Administration.Security=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security\Audit','Audit','Audit/Audit.aspx','Audit.png',NULL,NULL,'Forms\Audit',NULL,103,'NWR','U:Administration.Audit=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security\License','GUI.License','Security/License.aspx','License.png',NULL,NULL,'Forms\Passports',NULL,104,NULL,'U:Administration.Security=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security\LockDB','GUI.LockDB','Security/LockDB.aspx','LockDB.png',NULL,NULL,'Forms\Passports',NULL,101,NULL,'U:Administration.Security=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Security\Passports','GUI.Passports','Security/Passports.aspx','Passports.png',NULL,NULL,'Forms\Passports',NULL,102,NULL,'U:Administration.Security=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftControl','GUI.ShiftControl',NULL,'Presencia.ico',NULL,NULL,NULL,NULL,1201,'NWR',NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftControl\AbsencesStatus','GUI.Absences','Absences/AbsencesStatus.aspx','AbsencesStatus.png',NULL,NULL,'Feature\Absences',NULL,103,'NWR','U:Employees=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftControl\Analytics','GUI.CalendarAnalytics','Scheduler/AnalyticsScheduler.aspx','CalendarAnalytics.png',NULL,NULL,'Forms\Calendar',NULL,102,NULL,'U:Calendar=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftControl\Scheduler','GUI.Scheduler','Scheduler/Scheduler.aspx','Scheduler.png',NULL,NULL,'Forms\Calendar',NULL,101,'NWR','U:Calendar=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement','Gui.ShiftManagement',NULL,'ShiftManagement.png',NULL,NULL,NULL,NULL,1202,NULL,NULL) 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\AbsencesDocuments','GUI.AbsencesDocuments','Absences/DocumentsAbsences.aspx','DocumentsAbsences.png',NULL,NULL,'Feature\Absences',NULL,107,'NWR','U:DocumentsAbsences=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\Accruals','GUI.Accruals','Concepts/Concepts.aspx','Concepts.png',NULL,NULL,'Forms\Concepts',NULL,101,'NWR','U:Concepts.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\Assignments','Assignments','Assignments/Assignments.aspx','Assignment.png',NULL,NULL,'Feature\HRScheduling',NULL,104,NULL,'U:Assignments.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\Causes','GUI.Causes','Causes/Causes.aspx','Causes.png',NULL,NULL,'Forms\Causes',NULL,103,'NWR','U:Causes.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\KPI','KPI','Indicators/Indicators.aspx','Indicators.png',NULL,NULL,'Feature\KPIs',NULL,106,NULL,'U:KPI.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\Shifts','GUI.Shifts','Shifts/ShiftsExpress.aspx','Shifts.png',NULL,NULL,'Forms\ShiftsExpress',NULL,102,'NWR','U:Shifts.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\ShiftManagement\ShiftsPro','GUI.Shifts','Shifts/Shifts.aspx','Shifts.png',NULL,NULL,'Forms\Shifts',NULL,102,'NWR','U:Shifts.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Task','ProductiV',NULL,'Task.png',NULL,NULL,'Feature\Productiv',NULL,1401,NULL,'U:Tasks.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Task\Analytics','TaskAnalytics','Tasks/Analytics.aspx','TaskAnalytics.png',NULL,NULL,'Feature\Productiv',NULL,102,NULL,'U:Tasks.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Task\Tasks','Task.Status','Tasks/Tasks.aspx','Task.png',NULL,NULL,'Feature\Productiv',NULL,101,NULL,'U:Tasks.Definition=Read') 
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities]) VALUES ('Portal\Task\TaskTemplates','TaskTemplates','TaskTemplates/TaskTemplates.aspx','TaskTemplates.png',NULL,NULL,'Feature\Productiv',NULL,103,NULL,'U:Tasks.TemplatesDefinition=Read') 
GO

-- ExportGuides para las exportaciones XML
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8988,'Exportación calendario y puestos','',0,1,1,'','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8987,'Exportación calendario y puestos por nombres','',0,1,1,'','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8986,'Exportación de puestos','',0,1,2,';','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8984,'Exportación de vacaciones y horarios','',0,1,1,'','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8985,'Exportación de tareas por empleado','',0,1,1,'','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8983,'Exportación Labor','',0,1,1,'','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8982,'Exportación Logic Class','',0,1,2,';','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8981,'Exportación Iberper','',0,1,2,';','0,1,0@0@0')
GO
INSERT INTO [dbo].[ExportGuides] ([ID],[Name],[ProfileMask],[ProfileType],[Mode],[ExportFileType],[Separator],[DisplayParameters])
     VALUES (8980,'Exportación Horarios','',0,1,1,'','0,1,0@0@0')
GO

-- En Win32 no se configuraba el tipo de horario
update [dbo].[shifts] set [shifttype] = 1 where [shifttype] is NULL
GO

--Crear tabla gui actions
CREATE TABLE [dbo].[sysroGUI_Actions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDPath] [nvarchar](200) NOT NULL,
	[IDGUIPath] [nvarchar](200) NOT NULL,
	[LanguageTag] [nvarchar](64) NOT NULL,
	[RequieredFeatures] [nvarchar](200) NULL,
	[RequieredFunctionalities] [nvarchar](200) NULL,
	[AfterFunction] [nvarchar](max) NULL,
	[CssClass] [nvarchar](64) NULL,
	[Section] [int] NOT NULL,
	[ElementIndex] [int] NULL
) ON [PRIMARY]
GO

delete from [dbo].[sysroGUI_Actions]
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Access\AccessStatus\Child','tbMaximize','Forms\Access','U:Access.Zones.Supervision=Read','MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('ShowAccFilterParent','Portal\Access\AccessStatus\Child','tbAccFilter','Forms\Access','U:Access.Zones.Supervision=Read','ShowAccFilter(''#ID#'')','btnTbAccFilter2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\Access\AccessStatus\Child','tbShowReports','Forms\Access','U:Access.Zones.Supervision=Read','ShowReports','btnTbPrint2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('ShowParent','Portal\Access\AccessStatus\Child','tbViewParent','Forms\Access','U:Access.Zones.Supervision=Read','ReturnToParent()','btnTbShowZonePlane2',1,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('ViewCam','Portal\Access\AccessStatus\Child','tbAccViewCam','Forms\Access','U:Administration.Cameras.Visualization=Read','viewCam(''#IDCAMERA#'')','btnTbAccViewCam32',1,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('AccessMonitor','Portal\Access\AccessStatus\Child','tbMonitor','Forms\Access','U:Access.Zones.Supervision=Read','showAccessStatusMonitor','btnTbMonitor2',1,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('PlateFilter','Portal\Access\AccessStatus\Child','tbAccPlatesFilter','Feature\TerminalConnector',NULL,'ShowAccPlatesfilter()','btnTbAccPlatesFilter2',1,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Access\AccessStatus\Parent','tbMaximize','Forms\Access','U:Access.Zones.Supervision=Read','MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('AccessFilter','Portal\Access\AccessStatus\Parent','tbAccFilter','Forms\Access','U:Access.Zones.Supervision=Read','ShowAccFilter(null,null)','btnTbAccFilter2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('AccPlatesFitlter','Portal\Access\AccessStatus\Parent','tbAccPlatesFilter','Forms\Access','U:Access.Zones.Supervision=Read','ShowAccPlatesFilter()','btnTbAccPlatesFilter2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\Access\AccessStatus\Parent','tbShowReports','Forms\Access','U:Access.Zones.Supervision=Read','ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Access\Events\management','tbMaximize','Forms\Access',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\Access\Events\management','tbAddNewEventScheduler','Forms\Access','U:Events.Definition=Admin','newEventScheduler()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\Access\Events\management','tbDelEventScheduler','Forms\Access','U:Events.Definition=Admin','ShowRemoveEventScheduler(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Copy','Portal\Access\Events\management','tbCopyEventScheduler','Forms\Access','U:Events.Definition=Admin','copyEventScheduler(''#ID#'')','btnTbCopyEventScheduler',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\AccessManagement\AccessGroups\management','tbMaximize','Forms\Access',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\AccessManagement\AccessGroups\management','tbAddNewGroup','Forms\Access','U:Access.Groups.Definition=Admin','newAccessGroup()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\AccessManagement\AccessGroups\management','tbDelGroup','Forms\Access','U:Access.Groups.Definition=Admin','ShowRemoveAccessGroup(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Copy','Portal\AccessManagement\AccessGroups\management','tbCopyAccessGroup','Forms\Access','U:Access.Groups.Definition=Admin','copyAccessGroup(''#ID#'')','btnTbAddGroupBatch2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\AccessManagement\AccessGroups\management','tbShowReports','Forms\Access',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\AccessManagement\AccessPeriods\management','tbMaximize','Forms\Access',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\AccessManagement\AccessPeriods\management','tbAddNewPeriod','Forms\Access','U:Access.Periods.Definition=Admin','newAccessPeriod()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\AccessManagement\AccessPeriods\management','tbDelPeriod','Forms\Access','U:Access.Periods.Definition=Admin','ShowRemoveAccessPeriod(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\AccessManagement\AccessPeriods\management','tbShowReports','Forms\Access',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\AccessManagement\DiningRoom\management','tbMaximize','Forms\DiningRoom',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\AccessManagement\DiningRoom\management','tbAddNewDiningRoom','Forms\DiningRoom','U:DiningRoom.Turns=Admin','newDiningRoom()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\AccessManagement\DiningRoom\management','tbDelDiningRoom','Forms\DiningRoom','U:DiningRoom.Turns=Admin','ShowRemoveDiningRoom(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\AccessManagement\DiningRoom\management','tbShowReports','Forms\DiningRoom',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\CostControl\BusinessCenters\Management','tbMaximize',NULL,NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\CostControl\BusinessCenters\Management','tbAddNewBusinessCenter',NULL,'U:BusinessCenters.Definition=Admin','newBusinessCenter()','btnTbAdd2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\CostControl\BusinessCenters\Management','tbDelBusinessCenter',NULL,'U:BusinessCenters.Definition=Admin','ShowRemoveBusinessCenter(''#ID#'')','btnTbDel2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\General\Employees\Employees','tbMaximize','Forms\Employees',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeePhoto','Portal\General\Employees\Employees','tbAddEmployeeImage','Forms\Employees','U:Employees.NameFoto=Write','ShowChangeEmployeeImage(''#ID#'')','btnTbImg2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeePhotoWzd','Portal\General\Employees\Employees','tbWizardEmployeeMessage','Forms\Employees','U:Employees.NameFoto=Write','ShowWizardEmployeeMessage(''#ID#'')','btnTbMsg2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeeCopyWzd','Portal\General\Employees\Employees','tbEmployeeCopyWizard','Forms\Employees','U:Employees=Admin','ShowEmployeeCopyWizard(''#ID#'')','btnTbAddGroupBatch2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeeDelWzd','Portal\General\Employees\Employees','tbDelEmployee','Forms\Employees','U:Employees=Admin','ShowCaptchaRemoveEmployee(''#ID#'')','btnTbDel2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\General\Employees\Employees','tbShowReports','Forms\Employees',NULL,'ShowReports','btnTbPrint2',0,6)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewEmployee','Portal\General\Employees\Employees','tbAddNewEmployee','Forms\Employees','U:Employees=Admin','ShowNewEmployeeWizard()','btnTbAddEmp2',1,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeeSecurityActions','Portal\General\Employees\Employees','tbEmployeeActionsSecurity','Forms\Passports','U:Employees=Admin','ShowSecurityActionsWizard(''e'')','btnEmployeeActionsSecurity',1,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\General\Employees\Groups','tbMaximize','Forms\Employees',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('DeleteGroup','Portal\General\Employees\Groups','tbdelGroup','Forms\Employees','U:Employees=Admin','ShowRemoveGroup(''#ID#'')','btnTbDel2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeeMessage','Portal\General\Employees\Groups','tbWizardEmployeeMessage','Forms\Passports','U:Employees=Read','ShowWizardEmployeeMessage(''#ID#'')','btnTbMsg2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\General\Employees\Groups','tbShowReports','Forms\Employees',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewGroup','Portal\General\Employees\Groups','tbAddNewGroup','Forms\Employees','U:Employees.Groups=Admin','ShowNewGroupWizard()','btnTbAddGroup2',1,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewEmployee','Portal\General\Employees\Groups','tbAddNewEmployee','Forms\Employees','U:Employees.Groups=Admin','ShowNewEmployeeWizard()','btnTbAddEmp2',1,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewCompany','Portal\General\Employees\Groups','tbAddNewCompany','Feature\MultiCompany','U:Employees.Groups=Admin','ShowNewCompanyWizard()','btnTbAddCompany2',1,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('EmployeeSecurityActions','Portal\General\Employees\Groups','tbEmployeeActionsSecurity','Forms\Passports','U:Employees=Admin','ShowSecurityActionsWizard(''e'')','btnEmployeeActionsSecurity',1,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\General\Requests\Requests','tbShowReports',NULL,NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\AccessZones\management','tbMaximize',NULL,NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\AccessZones\management','tbAddNewZone',NULL,'U:Access.Zones.Definition=Admin','newAccessZone()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\GeneralManagement\AccessZones\management','tbDelZone',NULL,'U:Access.Zones.Definition=Admin','ShowRemoveAccessZone(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\AccessZones\management','tbShowReports',NULL,NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\AccessZones\Resume','tbMaximize',NULL,NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\AccessZones\Resume','tbAddNewZone',NULL,'U:Access.Zones.Definition=Admin','newAccessZone()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('ZoomPlanes','Portal\GeneralManagement\AccessZones\Resume','tbShowZonePlane',NULL,'U:Access.Zones.Definition=Admin','ShowZonePlanes()','btnTbShowZonePlane2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\AccessZones\Resume','tbShowReports',NULL,NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\Activities\Management','tbMaximize','Forms\Activities',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\Activities\Management','tbAddNewActivity','Forms\Activities','U:Activities.Definition=Admin','newActivity()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\GeneralManagement\Activities\Management','tbDelActivity','Forms\Activities','U:Activities.Definition=Admin','ShowRemoveActivity(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\Activities\Management','tbShowReports','Forms\Activities',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\Cameras\Management','tbMaximize','Version\Live',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\Cameras\Management','tbAddNewCamera','Version\Live','U:Administration.Cameras.Definition=Admin','newCamera()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\GeneralManagement\Cameras\Management','tbDelCamera','Version\Live','U:Administration.Cameras.Definition=Admin','ShowRemoveCamera(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\Cameras\Management','tbShowReports','Version\Live',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\Notifications\Management','tbMaximize','Process\Notifier',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\Notifications\Management','tbAddNewNotification','Process\Notifier','U:Administration.Notifications.Definition=Admin','newNotification()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\GeneralManagement\Notifications\Management','tbDelNotification','Process\Notifier','U:Administration.Notifications.Definition=Admin','ShowRemoveNotification(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\Notifications\Management','tbShowReports','Process\Notifier',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\ReportScheduler\Management','tbMaximize','Process\ReportServer',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\ReportScheduler\Management','tbAddNewReportScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','newReportScheduler()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\GeneralManagement\ReportScheduler\Management','tbDelReportScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','ShowRemoveReportScheduler(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Execute','Portal\GeneralManagement\ReportScheduler\Management','tbExecuteReportScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Write','ExecuteNow()','btnTbExecuteReport',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\ReportScheduler\Management','tbShowReports','Process\ReportServer',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\Terminal\List','tbMaximize','Forms\Terminals',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\Terminal\List','tbAddTerminal','Forms\Terminals','U:Terminals.Definition=Admin','AddNewTerminal()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Broadcaster','Portal\GeneralManagement\Terminal\List','tbLaunchBroadcaster','Forms\Terminals','U:Terminals.StatusInfo=Read','ConfirmLaunchBroadcaster','btnProgramTerminals',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('CommState','Portal\GeneralManagement\Terminal\List','tbEnableDisableComms','Forms\Terminals','U:Terminals.StatusInfo=Read','ChangeCommsState()','btnTbCommsState2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\Terminal\List','tbShowReports','Forms\Terminals',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\GeneralManagement\Terminal\Terminals','tbMaximize','Forms\Terminals',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\GeneralManagement\Terminal\Terminals','tbAddTerminal','Forms\Terminals','U:Terminals.Definition=Admin','AddNewTerminal()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\GeneralManagement\Terminal\Terminals','tbDelTerminal','Forms\Terminals','U:Terminals.Definition=Admin','ShowRemoveTerminal(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('BroadcasterTerminal','Portal\GeneralManagement\Terminal\Terminals','tbLaunchBroadcasterForTerminal','Forms\Terminals','U:Terminals.Definition=Admin','ConfirmLaunchBroadcasterForTerminal()','btnProgramSelectedTerminal',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\GeneralManagement\Terminal\Terminals','tbShowReports','Forms\Terminals',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\LabAgree\LabAgree\Management','tbMaximize','Feature\ConcertRules',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\LabAgree\LabAgree\Management','tbAddNewLabAgree','Feature\ConcertRules','U:LabAgree.Definition=Admin','newLabAgree()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\LabAgree\LabAgree\Management','tbDelLabAgree','Feature\ConcertRules','U:LabAgree.Definition=Admin','ShowRemoveLabAgree(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\LabAgree\LabAgree\Management','tbShowReports','Feature\ConcertRules',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\LabAgree\LabAgreeRules\Management','tbMaximize','Feature\ConcertRules',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\LabAgree\LabAgreeRules\Management','tbAddNewRule','Feature\ConcertRules','U:LabAgree.AccrualRules.Definition=Admin','newLabAgreeRule()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\LabAgree\LabAgreeRules\Management','tbDelRule','Feature\ConcertRules','U:LabAgree.AccrualRules.Definition=Admin','ShowRemoveLabAgreeRule(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\LabAgree\LabAgreeRules\Management','tbShowReports','Feature\ConcertRules',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\LabAgree\StartupValues\Management','tbMaximize','Feature\ConcertRules',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\LabAgree\StartupValues\Management','tbAddNewStartupValue','Feature\ConcertRules','U:LabAgree.StartupValues.Definition=Admin','newStartupValue()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\LabAgree\StartupValues\Management','tbDelStartupValue','Feature\ConcertRules','U:LabAgree.StartupValues.Definition=Admin','ShowRemoveStartupValue(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\LabAgree\StartupValues\Management','tbShowReports','Feature\ConcertRules',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Security\Passports\Management','tbMaximize','Forms\Passports',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\Security\Passports\Management','tbAddNewPassport','Forms\Passports','U:Administration.Security=Admin','ShowNewPassportWizard(''#ID#'')','btnTbAdd3',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\Security\Passports\Management','tbDelPassport','Forms\Passports','U:Administration.Security=Admin','ShowRemovePassport(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Move','Portal\Security\Passports\Management','tbMovePassports','Forms\Passports','U:Administration.Security=Admin','ShowEmployeesGroupWizard(''#ID#'')','btnTbMove3',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftControl\Scheduler\calendar','tbMaximize','Forms\Calendar',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Incomplete','Portal\ShiftControl\Scheduler\calendar','IncompletedDays','Forms\Calendar',NULL,'ShowIncompletedDays()','btnTbDispID2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Justified','Portal\ShiftControl\Scheduler\calendar','NotJustifiedDays','Forms\Calendar',NULL,'ShowNotJustifiedDays()','btnTbDispNJ2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reliabled','Portal\ShiftControl\Scheduler\calendar','NotReliabledDays','Forms\Calendar',NULL,'ShowNotReliabledDays()','btnTbDispNR2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('CopyEmp','Portal\ShiftControl\Scheduler\calendar','CopyPlanBtwEmployees','Forms\Calendar','U:Calendar.Scheduler=Write','ShowCopySchedulerWizard()','btnTbCopyP2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftControl\Scheduler\calendar','ShowReports','Forms\Calendar',NULL,'ShowReports','btnTbPrint2',0,6)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('WeekPlan','Portal\ShiftControl\Scheduler\calendar','WeekPlan','Forms\Calendar','U:Calendar.Scheduler=Write','ShowWeekScheduleWizard()','btnTbPlanS2',1,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Remarks','Portal\ShiftControl\Scheduler\calendar','RemarksConfig','Forms\Calendar','U:Calendar.Highlight=Read','ShowRemarksConfig()','btnTbRemarksConfig2',1,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Template','Portal\ShiftControl\Scheduler\calendar','TemplateAssign','Forms\Calendar',NULL,'ShowTemplateAssignWizard()','btnTbTemplateConfig2',1,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('AssignCause','Portal\ShiftControl\Scheduler\calendar','AssignCauses','Forms\Calendar',NULL,'ShowAssignCausesWizard()','btnTbMassCause',1,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('JustifiedIncicences','Portal\ShiftControl\Scheduler\calendar','JustifiedIncicences','Forms\Calendar',NULL,'ShowIncidencesWizard()','btnTbMassIncidence',1,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('CostCenters','Portal\ShiftControl\Scheduler\calendar','AssignCenters','Feature\CostControl','U:BusinessCenters.Punches=Write','ShowAssignCentersWizard()','btnTbMassAssignCenters',1,6)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MassPunch','Portal\ShiftControl\Scheduler\calendar','InsertMassPunch','Forms\Calendar',NULL,'ShowInsertMassPunchWizard()','btnTbMassPunch',1,6)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\AbsencesDocuments\management','tbMaximize','Feature\Absences',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\AbsencesDocuments\management','tbAddNewDocumentAbsence','Feature\Absences','U:DocumentsAbsences.Definition=Admin','newDocumentAbsence()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\AbsencesDocuments\management','tbDelDocumentAbsence','Feature\Absences','U:DocumentsAbsences.Definition=Admin','ShowRemoveDocumentAbsence(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\AbsencesDocuments\management','tbShowReports','Feature\Absences',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Accruals\Accruals','tbMaximize','Forms\Concepts',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Accruals\Accruals','tbAddNewConcept','Forms\Concepts','U:Concepts.Definition=Admin','newConcept()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\Accruals\Accruals','tbDelConcept','Forms\Concepts','U:Concepts.Definition=Admin','ShowRemoveConcept(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Accruals\Accruals','tbShowReports','Feature\Absences',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Accruals\Groups','tbMaximize','Forms\Concepts',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Accruals\Groups','tbAddNewConceptGroup','Forms\Concepts','U:Concepts.Definition=Admin','newConceptGroup()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\Accruals\Groups','tbDelConceptGroup','Forms\Concepts','U:Concepts.Definition=Admin','ShowRemoveConceptGroup(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Accruals\Groups','tbShowReports','Forms\Concepts',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Assignments\Management','tbMaximize','Feature\HRScheduling',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Assignments\Management','tbAddNewAssignment','Feature\HRScheduling','U:Assignments.Definition=Admin','newAssignment()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\Assignments\Management','tbDelAssignment','Feature\HRScheduling','U:Assignments.Definition=Admin','ShowRemoveAssignment(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Assignments\Management','tbShowReports','Feature\HRScheduling',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Causes\Management','tbMaximize','Forms\Causes',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Causes\Management','tbAddNewCause','Forms\Causes','U:Causes.Definition=Admin','newCause()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\Causes\Management','tbDelCause','Forms\Causes','U:Causes.Definition=Admin','ShowRemoveCause(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Causes\Management','tbShowReports','Forms\Causes',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\KPI\Management','tbMaximize','Feature\KPIs',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\KPI\Management','tbAddNewIndicator','Feature\KPIs','U:KPI.Definition=Admin','newIndicator()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Delete','Portal\ShiftManagement\KPI\Management','tbDelIndicator','Feature\KPIs','U:KPI.Definition=Admin','ShowRemoveIndicator(''#ID#'')','btnTbDel2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\KPI\Management','tbShowReports','Feature\KPIs',NULL,'ShowReports','btnTbPrint2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Shifts\GroupManagement','tbMaximize','Forms\Shifts',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Shifts\GroupManagement','tbAddNewShift','Forms\Shifts','U:Shifts.Definition=Admin','newShift()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewGroup','Portal\ShiftManagement\Shifts\GroupManagement','tbAddNewShiftGroup','Forms\Shifts','U:Shifts.Definition=Admin','ShowNewShiftGroupWizard()','btnTbAddShiftGroup2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\ShiftManagement\Shifts\GroupManagement','tbdelShiftGroup','Forms\Shifts','U:Shifts.Definition=Admin','ShowRemoveShiftGroup(''#ID#'')','btnTbDel2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Shifts\GroupManagement','tbShowReports','Forms\Shifts',NULL,'ShowReports','btnTbPrint2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\ShiftManagement\Shifts\management','tbMaximize','Forms\Shifts',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('New','Portal\ShiftManagement\Shifts\management','tbAddNewShift','Forms\Shifts','U:Shifts.Definition=Admin','newShift()','btnTbAdd2',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewGroup','Portal\ShiftManagement\Shifts\management','tbAddNewShiftGroup','Forms\Shifts','U:Shifts.Definition=Admin','ShowNewShiftGroupWizard()','btnTbAddShiftGroup2',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Copy','Portal\ShiftManagement\Shifts\management','tbCopyShift','Forms\Shifts','U:Shifts.Definition=Admin','copyShift()','btnTbCopyShift2',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\ShiftManagement\Shifts\management','tbDelShift','Forms\Shifts','U:Shifts.Definition=Admin','ShowRemoveShift(''#ID#'')','btnTbDel2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Reports','Portal\ShiftManagement\Shifts\management','tbShowReports','Forms\Shifts',NULL,'ShowReports','btnTbPrint2',0,6)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Task\TaskTemplates\Project','tbMaximize','Feature\Productiv',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewProject','Portal\Task\TaskTemplates\Project','tbAddNewProject','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','ShowNewProject()','btnTbAddTaskProject',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewTask','Portal\Task\TaskTemplates\Project','tbAddNewTaskTemplate','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','newTaskTemplate(''#ID#'')','btnTbAddTaskTemplate',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('LaunchWizard','Portal\Task\TaskTemplates\Project','tbTbLaunchWizardTemplate','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','LaunchTaskTemplateWizard()','btnTbLaunchWizardTemplate',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\Task\TaskTemplates\Project','tbdelTaskTemplateProject','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','ShowRemoveProject(''#ID#'')','btnTbDel2',0,5)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('MaxMinimize','Portal\Task\TaskTemplates\Task','tbMaximize','Feature\Productiv',NULL,'MaxMinimize','btnMaximize2',0,1)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewProject','Portal\Task\TaskTemplates\Task','tbAddNewProject','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','ShowNewProject()','btnTbAddTaskProject',0,2)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('NewTask','Portal\Task\TaskTemplates\Task','tbAddNewTaskTemplate','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','newTaskTemplate(''#ID#'')','btnTbAddTaskTemplate',0,3)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('LaunchWizard','Portal\Task\TaskTemplates\Task','tbTbLaunchWizardTemplate','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','LaunchTaskTemplateWizard()','btnTbLaunchWizardTemplate',0,4)
GO
INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('Del','Portal\Task\TaskTemplates\Task','tbdelTaskTemplateProject','Feature\Productiv','U:Tasks.TemplatesDefinition=Admin','ShowRemoveTaskTemplate(''#ID#'')','btnTbDel2',0,5)
GO

UPDATE dbo.sysroParameters SET Data='368' WHERE ID='DBVersion'
GO