------ EJECUTAR SOBRE BBDD PROPORCIONADA POR CONSULTORIA ------------------------
-- STARTER GUI
UPDATE sysroGUI SET Edition = 'Starter' 
GO
--Opciones generales, zonas
UPDATE sysroGUI SET Edition = '' WHERE IDPath = 'Portal\Configuration\Options' OR IDPath = 'Portal\Configuration\Zones' OR IDPath = 'Portal\Configuration\Cameras' 
OR IDPath = 'Portal\Configuration\Terminal' OR IDPath = 'Portal\Configuration\Routes'  OR IDPath = 'Portal\Configuration\PortalConfiguration'
GO

-- Eliminamos: Convenios, Horarios, Justificaciones, Saldos, KPIs, Unidades Productivas, Puestos
UPDATE sysroGUI SET Edition = '' WHERE IDPath LIKE 'Portal\ShiftManagement%'
GO
--GENIUS, Analiticas, Enlace de datos, Informes planificados, Informes solicitados
--UPDATE sysroGUI SET Edition = '' WHERE IDPath LIKE 'Portal\Reports\Genius' OR IDPath LIKE 'Portal\Reports\AnalyticScheduler' OR IDPath LIKE 'Portal\Reports\DataLink' OR IDPath LIKE 'Portal\Reports\ReportScheduler' OR IDPath LIKE 'Portal\Reports\TasksQueue' 
--Se incluye Genius en Starter
UPDATE sysroGUI SET Edition = '' WHERE IDPath LIKE 'Portal\Reports\AnalyticScheduler' OR IDPath LIKE 'Portal\Reports\DataLink' OR IDPath LIKE 'Portal\Reports\ReportScheduler' OR IDPath LIKE 'Portal\Reports\TasksQueue' 
GO
--SSO
UPDATE sysroGUI SET Edition = '' WHERE IDPath LIKE '%Portal\Configuration\SSO%'
GO
--HHRScheduling
UPDATE sysroGUI SET Edition = '' WHERE RequiredFeatures like '%HRScheduling%'
GO
--ProductiV
UPDATE sysroGUI SET Edition = '' WHERE RequiredFeatures like '%Feature\Productiv%'
GO
--Centros de coste
UPDATE sysroGUI SET Edition = '' WHERE IDPath like '%Portal\CostControl%'
GO
--Accesos
UPDATE sysroGUI SET Edition = '' WHERE IDPath like 'Portal\Access%' OR IDPath LIKE 'Portal\Configuration\AccessZones'
GO
--LivePortal
UPDATE sysroGUI SET Edition = '' WHERE IDPath like 'LivePortal%' 
GO
--Seguridades obsoletas
UPDATE sysroGUI SET Edition = '' WHERE IDPath like 'Portal\Company\Passports' OR IdPath Like 'Portal\Company\SecurityChart' OR IdPath Like 'Portal\Company\supervisors'
GO
UPDATE sysroGUI SET Edition = '' WHERE IDPath = 'Portal\Configuration\Notifications'
GO
UPDATE sysroGUI SET Edition = '' WHERE IDPath = 'Portal\Security\SDK'
GO
--Importar / Exportar
UPDATE sysroGUI SET RequiredFeatures = 'Feature\DatalinkBusiness' WHERE IDPath = 'Portal\Reports\DataLinkBusiness'
GO
--Botones importar/exportar en calendario
update sysroGUI_Actions set edition='' where AfterFunction like '%ImportPlanFromExcel%'
GO
update sysroGUI_Actions set edition='' where AfterFunction like '%ExportPlanToExcel%'
GO

--Quitamos import/export de PLanificación
UPDATE ExportGuides SET Edition = 'Starter' 
GO
UPDATE ExportGuides SET Edition = '' WHERE ID = 20003
GO
UPDATE ImportGuides SET Edition = '' WHERE ID = 21
GO

-- STARTER Features
UPDATE sysroFeatures SET Edition = 'Starter' 
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Planification.Requests.ShiftChange' OR Alias = 'Planification.Requests.ShiftExchange'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Visits%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Task%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Punches.Requests.CostCenterForgotten' OR Alias = 'Punches.Requests.ExternalParts' OR Alias = 'Punches.Requests.ExternalWorkWeekResume'
GO

--Supervisores
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Calendar.Requests.ShiftExchange'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Calendar.Punches.Requests.ExternalParts' OR Alias = 'Calendar.Punches.Requests.ExternalWorkWeekResume' 
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Calendar.Punches.Requests.CostCenterForgotten'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Employees.Assignments'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Access%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Budgets%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'BusinessCenters%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Assignments%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Causes%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Concepts%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Shifts%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'DiningRoom%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Events%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'KPI%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'ProductiveUnit%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Activities%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Terminals%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'LabAgree%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Administration.Cameras%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Calendar.DataLink.Exports.Assignments' OR Alias = 'Calendar.DataLink.Exports.CalendarAssignments' OR Alias = 'Calendar.DataLink.Exports.CalendarNameAssignments'
OR Alias LIKE 'Calendar.DataLink.Exports.Holidays%' OR Alias = 'Calendar.DataLink.Imports.Assignments' OR Alias = 'Calendar.DataLink.Imports.HRSchedule'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Calendar.Requests.ShiftChange%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Employees.BusinessCenters%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias = 'Employees.DataLink.Exports.AccrualsDay' OR Alias = 'Employees.DataLink.Exports.AccrualsPeriod' OR Alias = 'Employees.DataLink.Exports.AdvDinning'
OR Alias = 'Employees.DataLink.Exports.Iberper' OR Alias = 'Employees.DataLink.Exports.Labor' OR Alias = 'Employees.DataLink.Exports.LoginClass' OR Alias = 'Employees.DataLink.Exports.Sage' OR Alias = 'Employees.DataLink.Imports.Company'
OR Alias = 'Employees.DataLink.Exports.StdEmployees'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Calendar.Analytics%'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Employees.DataLink.Exports.AdvAbsence'
GO
UPDATE sysroFeatures SET Edition = '' WHERE Alias LIKE 'Calendar.DataLink%'
GO

--Starter Asistentes
UPDATE sysroGUI_Actions SET Edition = 'Starter' 
-- Asistentes de intrucción masiva de cualquier cosa en calendario
UPDATE sysroGUI_Actions SET Edition = '' WHERE CssClass like 'btnTbMass%' AND IDGUIPath like 'Portal\ShiftControl%'
GO
UPDATE sysroGUI_Actions SET Edition = '' WHERE CssClass like 'btnTbMassAbsence'
GO
UPDATE sysroGUI_Actions SET Edition = '' WHERE AfterFunction like 'ShowWizardEmployeeMessage%'
GO
UPDATE sysroGUI_Actions SET Edition = '' WHERE IDPath like 'ExportExcel' OR IDPath like 'ImportExcel'
GO

--Satarter Notificaciones
DELETE Notifications WHERE IDType IN (11,13,14,16,17,22,23,24,25,26,28,29,30,31,41,45,60,62,63,54,57,58,59,64,69,71,73)
GO
DELETE sysroNotificationTypes WHERE ID IN (11,13,14,16,17,22,23,24,25,26,28,29,30,31,41,45,60,62,63,54,57,58,59,64,66,69,71,73)
GO
-- Documentos
UPDATE DocumentTemplates SET IsSystem = 1
GO
UPDATE DocumentTemplates SET ShortName = 'con' WHERE id = 7
GO
UPDATE DocumentTemplates SET ShortName = 'alt' WHERE id = 6
GO
UPDATE DocumentTemplates SET ShortName = 'baj' WHERE id = 5
GO
UPDATE DocumentTemplates SET ShortName = 'jus' WHERE id = 3
GO

UPDATE Causes SET VisibilityPermissions = 0  WHERE Name = 'Horas extraordinarias' OR Name = 'Tiempo recuperado'
GO