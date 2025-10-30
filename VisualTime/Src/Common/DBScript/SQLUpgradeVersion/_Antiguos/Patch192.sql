-- Nuevos controles de Empleados: Accesos, Presencia, Producción, ...

-- Ordenamos botones de ACCESOS
update sysroGUI 
set priority = 200, IconURL='reportsaccess.ico'
where
IDPath = 'NavBar\Access\Reports'
GO
update sysroGUI 
set priority = 400, IconURL='groupsaccess.ico'
where
IDPath = 'NavBar\Access\AccessGroupsPeriods'
GO
update sysroGUI 
set priority = 500, IconURL='periodsaccess.ico'
where
IDPath = 'NavBar\Access\AccessPeriods'
GO
update sysroGUI 
set priority = 600, IconURL='ZonesAccess.ico'
where
IDPath = 'NavBar\Access\AccessZones'
GO
update sysroGUI 
set IconURL='StatusAccess.ico'
where
IDPath = 'NavBar\Access\AccessStatus'
GO


-- Ordenamos botones de PRESENCIA
update sysroGUI 
set priority = 600, IconURL='mens.ico'
where
IDPath = 'NavBar\FirstTab\Employees'
GO
update sysroGUI 
set priority = 620, IconURL='reports.ico'
where
IDPath = 'NavBar\FirstTab\ReportsCR'
GO
update sysroGUI 
set priority = 630, IconURL='calendar.ico'
where
IDPath = 'NavBar\FirstTab\Scheduler'
GO
update sysroGUI 
set priority = 640, IconURL='concepts.ico'
where
IDPath = 'NavBar\FirstTab\Concepts'
GO
update sysroGUI 
set priority = 660, IconURL='Shifts1.ico'
where
IDPath = 'NavBar\FirstTab\Shifts'
GO
delete from sysrogui where idpath = 'NavBar\FirstTab\Link'
GO
delete from sysrogui where idpath = 'NavBar\FirstTab\Reports'
GO
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\FirstTab\Causes','Causes','roFormCauses.vbd','causes.ico','3111111111111111111111111111111111111111',680,'NWR','Forms\Causes')
GO



-- Ordenamos botones de PRODUCCIÓN
update sysroGUI 
set priority = 705
where
IDPath = 'NavBar\Job\JobsLive' 
GO
update sysroGUI 
set priority = 710 , IconURL='JobStatus.ico'
where
IDPath = 'NavBar\Job\JobStatus'
GO
update sysroGUI 
set priority = 720 , IconURL='JobEmployees.ico'
where
IDPath = 'NavBar\Job\Teams'
GO
update sysroGUI 
set priority = 760, IconURL='JobMachine.ico'
where
IDPath = 'NavBar\Job\Machines' 
GO
update sysroGUI 
set priority = 780, IconURL='reports.ico'
where
IDPath = 'NavBar\Job\Reports'
GO
update sysroGUI 
set priority = 800, IconURL='JobOrder.ico'
where
IDPath = 'NavBar\Job\JobTemplates'
GO
update sysroGUI 
set priority = 820, IconURL='JobIncidence.ico'
where
IDPath = 'NavBar\Job\JobIncidences'
GO

-- Cambio de iconos de CONFIGURACIÓN
update sysroGUI 
set IconURL='ExportOptions.ico'
where
IDPath = 'NavBar\Config\Exports'
GO
update sysroGUI 
set IconURL='Option.ico'
where
IDPath = 'NavBar\Config\Options'
GO
update sysroGUI 
set IconURL='Terminals.ico'
where
IDPath = 'NavBar\Config\Terminals'
GO
update sysroGUI 
set IconURL='Audit.ico'
where
IDPath = 'NavBar\Config\Audit'
GO
update sysroGUI 
set IconURL='User.ico'
where
IDPath = 'NavBar\Config\Users'
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='192' WHERE ID='DBVersion'
GO

