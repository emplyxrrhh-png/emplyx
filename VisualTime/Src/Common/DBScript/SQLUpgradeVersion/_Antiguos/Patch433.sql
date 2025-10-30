-- Vista de soporte: Empleados con autorización de accesos en terminales
 CREATE VIEW [dbo].[sysrovwEmployeesAccessAuthorizationsOnTerminals]
 AS
	SELECT dbo.Employees.ID as EmployeeID, TerminalReaders.IDTerminal, TerminalReaders.id IDReader
	FROM Employees
	INNER JOIN EmployeeContracts On EmployeeContracts.IDEmployee = Employees.ID
	And  dbo.EmployeeContracts.BeginDate < GETDATE() And dateadd(d,1,dbo.EmployeeContracts.EndDate) > GETDATE()
	LEFT OUTER JOIN sysrovwAccessAuthorizations On sysrovwAccessAuthorizations.IDEmployee = Employees.ID
	INNER JOIN (Select DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions, sysrovwAccessAuthorizations where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP On sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup 
	INNER JOIN TerminalReaders On TerminalReaders.IDZone=AGP.IDZone
GO

ALTER TABLE dbo.Terminals ALTER COLUMN Other varchar(500)
GO


--198: Inclusión de botón para cerrar sesión en la pantalla inicial de VTPortal
insert into sysroLiveAdvancedParameters values('VTPortalShowLogoutHome', 'false')
GO

--192: Notificación de cambio de planificación por defecto activada
update sysroLiveAdvancedParameters set value = 1 where ParameterName='Notifications.AllowAssignShift'
go

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'NewAna' AND IDGUIPath = 'Portal\GeneralManagement\ReportScheduler\Management')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES ('NewAna','Portal\GeneralManagement\ReportScheduler\Management','tbAddNewReportAnalitycsScheduler','Process\ReportServer','U:Administration.ReportScheduler.Definition=Admin','newReportAnalitycsScheduler()','btnTbAddAnalitycsReport',0,4)
GO

update dbo.sysroGUI_Actions set ElementIndex = 5 where IDPath='Del' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO
update dbo.sysroGUI_Actions set ElementIndex = 6 where IDPath='Execute' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO
update dbo.sysroGUI_Actions set ElementIndex = 7 where IDPath='Reports' and IDGUIPath='Portal\GeneralManagement\ReportScheduler\Management'
GO

ALTER TABLE [dbo].[ReportsScheduler] ADD 
	[Parameters] [nvarchar](max) NULL
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Assign' AND IDGUIPath = 'Portal\AISchedule\Budget\Schedule')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
		VALUES ('Assign', 'Portal\AISchedule\Budget\Schedule','tbAssignAISchedule','Feature\HRScheduling',NULL,'RunAIPlanner(true)','btnTbRunAIPlanner2',0,1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Remove' AND IDGUIPath = 'Portal\AISchedule\Budget\Schedule')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
		VALUES ('Remove', 'Portal\AISchedule\Budget\Schedule','tbRemoveAISchedule','Feature\HRScheduling',NULL,'RunAIPlanner(false)','btnTbRemoveAIPlanner2',0,2)
GO

UPDATE [dbo].[sysroGUI_Actions] SET ElementIndex = 3 WHERE [IDGUIPath] = 'Portal\AISchedule\Budget\Schedule' AND [IDPath] = 'Reports'
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 6 WHERE ParameterName='VTPortalApiVersion'
GO

UPDATE [dbo].[TerminalReaders] set ValidationMode = 'Local' where idterminal in (select id from [dbo].[Terminals] where type = 'MX8')
GO

UPDATE dbo.sysroParameters SET Data='433' WHERE ID='DBVersion'
GO
