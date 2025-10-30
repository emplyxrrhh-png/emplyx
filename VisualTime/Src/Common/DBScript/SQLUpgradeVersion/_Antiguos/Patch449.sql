IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroQueries] WHERE Name = 'Permisos efectivos')
	insert into dbo.sysroQueries(Name,Description,Value) values ('Permisos efectivos','Mustra los permisos de los supervisores en V2',
	'select tmp.SupervisorName,tmp.Feature, tmp.EmployeeName, CASE when tmp.Permission = 9 Then ''Administración'' when tmp.Permission = 6 Then ''Escritura'' when tmp.Permission = 3 then ''Lectura'' when tmp.Permission = 0 then ''Sin permiso'' else ''Sin permiso'' end as Permission from(
	select srp.Name as SupervisorName, srf.Alias As Feature, emp.Name as EmployeeName, dbo.WebLogin_GetPermissionOverEmployee(srp.ID, emp.ID, srf.EmployeeFeatureID,0,1, GETDATE()) as Permission from sysroPassports srp,sysroFeatures srf, Employees emp
	where srp.IsSupervisor = 1 and srf.Type = ''U'' and charindex(''@@ROBOTICS@@'',srp.Description) = 0 and srf.EmployeeFeatureID is not null and srf.EmployeeFeatureID in(1,2) and srf.AppHasPermissionsOverEmployees = 1 ) tmp
	where tmp.Permission > 0
	order by tmp.SupervisorName, tmp.Feature')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='449' WHERE ID='DBVersion'
GO