ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee] 
   	(
   		@idPassport int,
   		@idEmployee int,
   		@idApplication int,
   		@mode int, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
   		@includeGroups bit, --Esto no se utiliza pero se mantiene para que la definición de la función sea la misma
   		@date datetime
   	)
   RETURNS int
   AS
   BEGIN
   	DECLARE @Result int
 	DECLARE @parentPassport int
   	DECLARE @GroupType nvarchar(50)
  	
 	SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
   		
   	if @GroupType = 'U'
   	begin
   		SET @parentPassport = @idPassport
   	end
   	else
   	begin
   		SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
   	end
 	--SELECT @Result = isnull(sysroPermissionsOverEmployeesExceptions.Permission,sysroPermissionsOverGroups.Permission) FROM sysroPermissionsOverGroups
 	--	inner join sysrovwAllEmployeeMobilities on sysroPermissionsOverGroups.EmployeeGroupID=sysrovwAllEmployeeMobilities.IDGroup and @date between BeginDate and EndDate and sysrovwAllEmployeeMobilities.IDEmployee = @idEmployee
 	--	left outer join sysroPermissionsOverEmployeesExceptions on sysrovwAllEmployeeMobilities.IDEmployee=sysroPermissionsOverEmployeesExceptions.EmployeeID 
 	--	and sysroPermissionsOverEmployeesExceptions.PassportID = @parentPassport  and sysroPermissionsOverEmployeesExceptions.EmployeeFeatureID = @idApplication
 	--	where sysroPermissionsOverGroups.PassportID = @parentPassport  and sysroPermissionsOverGroups.EmployeeFeatureID = @idApplication

	 SELECT @Result = isnull(sysroPermissionsOverEmployeesExceptions.Permission,sysroPermissionsOverGroups.Permission) FROM sysroPermissionsOverGroups
 		inner join (select sysrovwAllEmployeeMobilities.*, eg.StartupDate from sysrovwAllEmployeeMobilities 
			inner join(select IDEmployee, min(beginDate) as StartupDate from sysrovwAllEmployeeMobilities where BeginDate > @date or @date between BeginDate and EndDate group by IDEmployee)eg
			on eg.IDEmployee = sysrovwAllEmployeeMobilities.IDEmployee) em
			on sysroPermissionsOverGroups.EmployeeGroupID=em.IDGroup and ((@date between em.BeginDate and em.EndDate) or em.StartupDate >@date) and em.IDEmployee = @idEmployee
		left outer join sysroPermissionsOverEmployeesExceptions on em.IDEmployee=sysroPermissionsOverEmployeesExceptions.EmployeeID 
 		and sysroPermissionsOverEmployeesExceptions.PassportID = @parentPassport and sysroPermissionsOverEmployeesExceptions.EmployeeFeatureID =  @idApplication
 		where sysroPermissionsOverGroups.PassportID = @parentPassport and sysroPermissionsOverGroups.EmployeeFeatureID =  @idApplication
		 			
 	RETURN @Result
   END
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='336' WHERE ID='DBVersion'
GO


