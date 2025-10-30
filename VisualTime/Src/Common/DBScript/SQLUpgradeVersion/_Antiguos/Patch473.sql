  
 alter PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverEmployeesExceptions]    
    (    
     @idPassport int    
    )    
       AS    
       BEGIN    
    DECLARE @pidPassport int = @idPassport      

MERGE sysroPermissionsOverEmployeesExceptions t1
USING sysroPassports_PermissionsOverEmployees t2
	ON t1.PassportID = t2.idpassport and t1.EmployeeID = t2.idemployee and t1.employeefeatureid = t2.idapplication 
WHEN MATCHED and t2.idpassport = @pidPassport THEN
  UPDATE
  SET t1.permission = t2.permission
WHEN NOT MATCHED  by target and t2.idpassport = @pidPassport then
	INSERT (PassportID, EmployeeID, EmployeeFeatureID, Permission)
	VALUES (t2.idpassport, t2.idemployee, t2.idapplication, t2.permission)
when not matched by source and t1.passportid = @pidPassport then
	delete;

END
GO

alter PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverEmployeesExceptions]  
    AS  
     BEGIN  
MERGE sysroPermissionsOverEmployeesExceptions t1
USING sysroPassports_PermissionsOverEmployees t2
	ON t1.PassportID = t2.idpassport and t1.EmployeeID = t2.idemployee and t1.employeefeatureid = t2.idapplication 
WHEN MATCHED  THEN
  UPDATE
  SET t1.permission = t2.permission
WHEN NOT MATCHED  by target  then
	INSERT (PassportID, EmployeeID, EmployeeFeatureID, Permission)
	VALUES (t2.idpassport, t2.idemployee, t2.idapplication, t2.permission)
when not matched by source then
	delete;
        
END
GO

-- se vuelven a generar los fullgroupname por un bug en versiones anteriores al guardar nuevos grupos
update [dbo].[groups]
set FullGroupName= dbo.GetFullGroupPathName(id)
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE  name = 'EmployeesWithContractOnGroup' AND type = 'TF')
BEGIN
	DROP FUNCTION [dbo].[EmployeesWithContractOnGroup]
END 
GO

CREATE FUNCTION [dbo].[EmployeesWithContractOnGroup]
  (				
	@datebeginpar smalldatetime,
	@dateendpar smalldatetime,
	@idgrouppar int,
	@recursivepar bit
  )
  RETURNS @ValueTable table(IDEmployee int, IDGroup int, DateBegin smalldatetime, DateEnd smalldatetime, Path nvarchar(100)) 
  AS
  BEGIN
    declare @iniperiod smalldatetime 
	declare @endperiod smalldatetime
	declare @idgroup int
	declare @recursive bit

	SET @iniperiod = @datebeginpar
	SET @endperiod = @dateendpar
	SET @idgroup = @idgrouppar
	SET @recursive = @recursivepar

  	INSERT INTO @ValueTable
	select IDEmployee, IDGroup, case when @iniperiod >= temp.mindate then @iniperiod else temp.mindate end as BeginDate, case when @endperiod >= temp.maxdate then temp.maxdate else @endperiod end as EndDate, temp.Path  
	from 
	(
	select ec.idemployee, aeg.idgroup, case when ec.BeginDate >= aeg.begindate then ec.BeginDate else aeg.begindate end as mindate, case when ec.EndDate >= aeg.Enddate then aeg.Enddate else ec.enddate end as maxdate, aeg.Path 
	from dbo.EmployeeContracts ec
	inner join dbo.sysrovwAllEmployeeMobilities aeg on 
			ec.IDEmployee = aeg.idemployee 
		and ec.BeginDate <= aeg.enddate 
		and ec.EndDate >= aeg.begindate 
		and (aeg.idgroup= @idgroup OR aeg.path like (cast((select Path from Groups where id= @idgroup)  as varchar) + cast('\%' as varchar)) or @idgroup=-1)
	) temp
	where @iniperiod <= temp.maxdate and @endperiod >= temp.mindate
	IF (@recursive <> 1 and @idgroup<>-1) DELETE @ValueTable WHERE IDGroup <> @idgroup
	RETURN
  END
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='473' WHERE ID='DBVersion'
GO
