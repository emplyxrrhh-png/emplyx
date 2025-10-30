update dbo.sysroGUI set RequiredFunctionalities='U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read OR U:Tasks.Requests.Forgotten=Read' where IDPath='Portal\General\Requests'
GO

ALTER TABLE dbo.ReportsScheduler ADD ReportType smallint NULL DEFAULT(0)
GO

UPDATE dbo.ReportsScheduler SET ReportType=0 WHERE ReportType IS NULL
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath]
           ,[IDGUIPath]
           ,[LanguageTag]
           ,[RequieredFeatures]
           ,[RequieredFunctionalities]
           ,[AfterFunction]
           ,[CssClass]
           ,[Section]
           ,[ElementIndex])
     VALUES
           ('NewEmp','Portal\GeneralManagement\ReportScheduler\Management','tbAddNewReportEmployeeScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','newReportEmployeeScheduler()','btnTbAddEmployeeReport',0,3)
GO

UPDATE [dbo].[sysroGUI_Actions] Set [CssClass] = 'btnTbAddSupervisorReport' WHERE [IDPath] = 'New' AND [IDGUIPath] = 'Portal\GeneralManagement\ReportScheduler\Management'
GO

update dbo.sysroGUI_Actions set ElementIndex = 4 where IDPath='Del' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO
update dbo.sysroGUI_Actions set ElementIndex = 5 where IDPath='Execute' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO
update dbo.sysroGUI_Actions set ElementIndex = 6 where IDPath='Reports' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO

alter table dbo.TMPANNUALINDIVIDUALCALENDARByContract add
Colordia1 int null default(0),
Colordia2 int null default(0),
Colordia3 int null default(0),
Colordia4 int null default(0),
Colordia5 int null default(0),
Colordia6 int null default(0),
Colordia7 int null default(0),
Colordia8 int null default(0),
Colordia9 int null default(0),
Colordia10 int null default(0),
Colordia11 int null default(0),
Colordia12 int null default(0),
Colordia13 int null default(0),
Colordia14 int null default(0),
Colordia15 int null default(0),
Colordia16 int null default(0),
Colordia17 int null default(0),
Colordia18 int null default(0),
Colordia19 int null default(0),
Colordia20 int null default(0),
Colordia21 int null default(0),
Colordia22 int null default(0),
Colordia23 int null default(0),
Colordia24 int null default(0),
Colordia25 int null default(0),
Colordia26 int null default(0),
Colordia27 int null default(0),
Colordia28 int null default(0),
Colordia29 int null default(0),
Colordia30 int null default(0),
Colordia31 int null default(0)
GO

IF EXISTS (SELECT 1 FROM dbo.sysroLiveAdvancedParameters WHERE ParameterName = 'VTPortalApiVersion')
	UPDATE dbo.sysroLiveAdvancedParameters SET Value = 1 WHERE ParameterName = 'VTPortalApiVersion'
ELSE
	INSERT INTO dbo.sysroLiveAdvancedParameters VALUES ('VTPortalApiVersion',1)
GO

IF EXISTS (SELECT 1 FROM dbo.sysroLiveAdvancedParameters WHERE ParameterName = 'ImportSecondaryKeyIsNIF')
	UPDATE dbo.sysroLiveAdvancedParameters SET Value = 1 WHERE ParameterName = 'ImportSecondaryKeyIsNIF'
ELSE
	INSERT INTO dbo.sysroLiveAdvancedParameters VALUES ('ImportSecondaryKeyIsNIF',1)
GO


UPDATE dbo.sysroParameters SET Data='411' WHERE ID='DBVersion'
GO
