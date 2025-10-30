-- Vista para la conversion de permisos a texto para analitica
create VIEW [dbo].[sysrovwPermisionEnum]
AS
select 9 as Permission ,'Administración' as StrPermission, 'A' as ChrPermission
union 
select 6 ,'Escritura' ,'W'
union
select 3, 'Lectura' , 'R'
union  
select 0, 'Sin permiso' , ''
union
select null, 'Sin permiso', ''
GO

CREATE VIEW [dbo].[sysrovwParentPassports]
AS
SELECT        ID as idPassport
			, CASE WHEN isnull(GroupType, '') = 'U' THEN id ELSE IDParentPassport END AS idParentPassport
FROM            dbo.sysroPassports
WHERE        (ISNULL(GroupType, '') = 'U') OR
                         (NOT (IDParentPassport IS NULL))
GO

CREATE VIEW [dbo].[sysrovwGetCurrentPermissionOverEmployee]
 AS
  	SELECT pp.idPassport as PassportID, pp.IDParentPassport as ParentPassportID, pog.EmployeeFeatureID,pog.EmployeeGroupID
	,ceg.IDEmployee as EmployeeID,pog.Permission,pog.LevelOfAuthority  
	,isnull(poe.Permission  ,9) as EmployeesExceptionsPermission
	, CASE WHEN pog.Permission > isnull(poe.Permission  ,9)
		THEN isnull(poe.Permission  ,9)
		ELSE pog.Permission END as CalculatedPermission
  		FROM sysrovwParentPassports pp
		inner join sysroPermissionsOverGroups  pog
		on pp.IDParentPassport=pog.PassportID 
		LEFT OUTER JOIN sysrovwCurrentEmployeeGroups ceg
		ON pog.EmployeeGroupID = ceg.IDGroup 
		LEFT OUTER JOIN sysroPermissionsOverEmployeesExceptions poe 
		ON ceg.IDEmployee = poe.EmployeeID 
		AND poe.EmployeeFeatureID = pog.EmployeeFeatureID
		and poe.PassportID = pog.PassportID 
GO

CREATE VIEW [dbo].[sysrovwGetPermissionOverEmployee]
AS
  	SELECT pp.idPassport as PassportID, pp.IDParentPassport as ParentPassportID, pog.EmployeeFeatureID,pog.EmployeeGroupID
	,eg.IDEmployee as EmployeeID,pog.Permission,pog.LevelOfAuthority  
	,isnull(poe.Permission  ,9) as EmployeesExceptionsPermission
	, CASE WHEN pog.Permission > isnull(poe.Permission  ,9)
		THEN isnull(poe.Permission  ,9)
		ELSE pog.Permission END as CalculatedPermission
	, eg.BeginDate, eg.EndDate 
  	FROM sysrovwParentPassports pp
	inner join sysroPermissionsOverGroups  pog
	on pp.IDParentPassport=pog.PassportID 
	LEFT OUTER JOIN EmployeeGroups eg
	ON pog.EmployeeGroupID = eg.IDGroup 
	LEFT OUTER JOIN sysroPermissionsOverEmployeesExceptions poe 
	ON eg.IDEmployee  = poe.EmployeeID 
	AND poe.EmployeeFeatureID = pog.EmployeeFeatureID
	and poe.PassportID = pog.PassportID 
GO

CREATE VIEW [dbo].[sysrovwEmployeeUserFieldValues]
AS
 
select efv1.[IDEmployee],efv1.FieldName,CONVERT(NVARCHAR(4000),efv1.[Value]) as [Value],efv1.[Date] as BeginDate, isnull(dateadd(n,-1,efv2.Date),convert(smalldatetime,'2079-01-01',120)) as EndDate
  from (SELECT [IDEmployee],FieldName,[Value],[Date]
	  , ROW_NUMBER() over (partition by [IDEmployee], fieldname order by date desc) as rowid
  FROM [EmployeeUserFieldValues]
  where [date]<getdate()
  ) efv1
  left outer join 
  (SELECT [IDEmployee],FieldName,[Value],[Date]
	  , ROW_NUMBER() over (partition by [IDEmployee], fieldname order by date desc) as rowid
  FROM [EmployeeUserFieldValues]
  where [date]<getdate()
  ) efv2
  on efv1.IDEmployee=efv2.IDEmployee 
  and efv1.FieldName=efv2.FieldName
  and efv1.rowid-1 = efv2.rowid 
 GO

update dbo.sysroQueries
set value='
select tmp1.SupervisorName, tmp1.EmployeeName
, isnull(EmployeePermission,''Sin permiso'') as ''F:Empleados''
, isnull(CalendarPermission,''Sin permiso'') as ''F:Calendario''
from (select srp.Name as SupervisorName,  emp.Name as EmployeeName
	, ps.StrPermission as EmployeePermission
		, srp.ID as IDPassport, emp.id as IDEmployee
		from sysroPassports srp
		left outer join sysrovwGetCurrentPermissionOverEmployee poe
		on poe.EmployeeFeatureID=1 
		and poe.passportid=srp.id 
		inner join [sysrovwPermisionEnum] ps
		on poe.CalculatedPermission=ps.Permission
		inner join Employees emp
		on poe.EmployeeID=emp.ID 
		where srp.IsSupervisor = 1 
			and  charindex(''@@ROBOTICS@@'',srp.Description) = 0 
			and poe.CalculatedPermission > 0) tmp1
full outer join (
	select srp.Name as SupervisorName,  emp.Name as EmployeeName
	, ps.StrPermission as CalendarPermission
		, srp.ID as IDPassport, emp.id as IDEmployee
		from sysroPassports srp
		left outer join sysrovwGetCurrentPermissionOverEmployee poe
		on poe.EmployeeFeatureID=2 
		and poe.passportid=srp.id 
		inner join [sysrovwPermisionEnum] ps
		on poe.CalculatedPermission=ps.Permission
		inner join Employees emp
		on poe.EmployeeID=emp.ID 
		where srp.IsSupervisor = 1 
			and  charindex(''@@ROBOTICS@@'',srp.Description) = 0 
			and poe.CalculatedPermission > 0) tmp2
on tmp1.IDPassport=tmp2.IDPassport and tmp1.IDEmployee = tmp2.IDEmployee 
order by tmp1.SupervisorName, tmp1.EmployeeName
'
WHERE Name = 'Permisos efectivos'
GO

update sysroLiveAdvancedParameters set Value='8' where ParameterName='VTPortalApiVersion'
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='462' WHERE ID='DBVersion'
GO
