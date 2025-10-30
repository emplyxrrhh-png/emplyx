update sysroGUI set RequiredFunctionalities ='E:Punches.Requests=Read OR E:Planification.Requests=Read OR E:UserFields.Requests=Read OR E:TaskPunches.Query=Read' where IDPath = 'LivePortal\MyRequests'
GO
update sysroGUI set RequiredFunctionalities ='U:Employees.UserFields.Requests=Read OR U:Calendar.Punches.Requests=Read OR U:Calendar.Requests=Read OR U:Tasks.Requests.Forgotten=Read' where IDPath = 'Portal\General\Requests'
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='320' WHERE ID='DBVersion'
GO

