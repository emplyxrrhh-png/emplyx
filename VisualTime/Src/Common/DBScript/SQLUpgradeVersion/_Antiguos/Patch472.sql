
alter VIEW [dbo].[sysrovwAllEmployeeGroups]    
  AS    
  SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate    
  , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled    
  , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,     
      sysrovwCurrentEmployeeGroups.isTransfer,'ERROR' As CostCenter    
  FROM            dbo.sysrovwGetEmployeeGroup geg    
  INNER JOIN dbo.sysrovwCurrentEmployeeGroups    
  ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee and getdate() between geg.BeginDate and geg.EndDate    
  UNION    
  SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate    
  , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled    
  , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,     
      sysrovwFutureEmployeeGroups.isTransfer,'ERROR' As CostCenter    
  FROM            dbo.sysrovwGetEmployeeGroup geg    
  INNER JOIN dbo.sysrovwFutureEmployeeGroups     
   ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee and geg.RealBeginDate > getdate() 
GO

-- Permiso sobre las observaciones diarias en edicion de fichajes
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =2900)
BEGIN
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID]) VALUES (2900,2,'Calendar.Remarks','Observaciones','','U','RWA',NULL,NULL,2)
INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 2900, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 2

INSERT INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures] SELECT [IDGroupFeature], 2900, [Permision]  FROM [dbo].[sysroGroupFeatures_PermissionsOverFeatures] WHERE [IDFeature] = 2
END
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

-- Funcion para obtener el total de empleados en un nivel departamental concreto
CREATE FUNCTION [dbo].[GetNumberEmployeesFromGroupLevel](  
   @FULLGROUPNAME   varchar(8000)  
  ,@LEVEL    int  
  ,@BEGINDATE    smalldatetime
  ,@ENDDATE    smalldatetime
  )  
  RETURNS int
  AS  
    BEGIN 
	  DECLARE @Result int
      DECLARE @pFULLGROUPNAME      varchar(8000)  = @FULLGROUPNAME
	  DECLARE @pLEVEL    int   = @LEVEL
   	  DECLARE @pTEXT      varchar(8000)  = ''
	  DECLARE @pLEVELTEXT      varchar(8000)  = ''
	  DECLARE @pBEGINDATE    smalldatetime = @BEGINDATE
	  DECLARE @pENDDATE    smalldatetime = @ENDDATE
      DECLARE @POS_START  int = 1  

      WHILE (@pLEVEL >0 AND @POS_START <= @pLEVEL)  
      BEGIN  
		SET @pLEVELTEXT = dbo.UFN_SEPARATES_COLUMNS(@pFULLGROUPNAME,@POS_START,'\')
		IF (LEN(@pLEVELTEXT) = 0)
		BEGIN
			SET @POS_START =  @pLEVEL + 2
		END
		ELSE
		BEGIN
			IF (LEN(@pTEXT) = 0)
			BEGIN
				SET @pTEXT = @pLEVELTEXT
			END
			ELSE
			BEGIN
				SET @pTEXT = @pTEXT + '\' + @pLEVELTEXT
			END
		END
		SET @POS_START = @POS_START + 1  
	  END

	  SET @Result = 0

	  IF LEN(@pTEXT) > 0 and @POS_START = (@pLEVEL +1)
	  BEGIN
		  SELECT @Result = count(distinct IDEmployee) FROM EmployeeGroups with (nolock) WHERE
	                ((EmployeeGroups.BeginDate <=  @pBEGINDATE  And EmployeeGroups.EndDate >=  @pBEGINDATE ) Or 
                         (EmployeeGroups.BeginDate <=  @pENDDATE  And EmployeeGroups.EndDate >= @pENDDATE ) Or 
                         (EmployeeGroups.BeginDate >  @pBEGINDATE  And EmployeeGroups.EndDate <  @pENDDATE )) 
                         And (SELECT COUNT(*) FROM EmployeeContracts  with (nolock) WHERE EmployeeContracts.IDEmployee = EmployeeGroups.IDEmployee 
                         And ((EmployeeContracts.BeginDate <=  @pBEGINDATE  And EmployeeContracts.EndDate >=  @pBEGINDATE ) Or 
                         (EmployeeContracts.BeginDate <=  @pENDDATE  And EmployeeContracts.EndDate >=  @pENDDATE ) Or 
                         (EmployeeContracts.BeginDate >  @pBEGINDATE  And EmployeeContracts.EndDate <  @pENDDATE ))) > 0 
							AND IDGroup IN(select ID FROM Groups with (nolock) where (FullGroupName like rtrim(@pTEXT) + ' \%') or (FullGroupName like rtrim(@pTEXT) )) 
	  END
    RETURN @Result
END
GO

ALTER TABLE dbo.ExportGuides ADD [DatasourceFile] [nvarchar](50) NOT NULL DEFAULT('')
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
		and (aeg.idgroup= @idgroup OR aeg.path like concat((select Path from Groups where id= @idgroup),'\%') or @idgroup=-1)
	) temp
	where @iniperiod <= temp.maxdate and @endperiod >= temp.mindate
	IF (@recursive <> 1 and @idgroup<>-1) DELETE @ValueTable WHERE IDGroup <> @idgroup
	RETURN
  END
GO

ALTER PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverEmployeesExceptions]    
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



-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='472' WHERE ID='DBVersion'
GO
