ALTER TABLE dbo.ExportGuides ADD
   RequieredFunctionalities nvarchar(200) NULL
GO
 
 CREATE FUNCTION [dbo].[GetEmployeeBusinessCenterOnDate]
  (	
 	@idEmployee int,
	@date datetime
  )
  RETURNS int
  AS
  BEGIN
    	DECLARE @businessCenter int
		select @businessCenter = 
    	(select top 1 BusinessCenters.ID 
                from Groups 
                inner join dbo.GetEmployeeGroupTree(@idEmployee,null,@date) eg on Groups.ID = eg.ID 
                inner join BusinessCenters on Groups.IDCenter = BusinessCenters.id 
                WHERE IDCenter is not null order by Path DESC)
    	   		   
    	RETURN @businessCenter
  END
GO

  CREATE FUNCTION [dbo].[GetEmployeeBusinessCenter]
  (	
 	@idEmployee int
  )
  RETURNS int
  AS
  BEGIN
    	DECLARE @businessCenter nvarchar(64)
		select @businessCenter = [dbo].[GetEmployeeBusinessCenterOnDate](@idEmployee, getdate())

		RETURN @businessCenter
  END
GO


  
  CREATE FUNCTION [dbo].[GetEmployeeBusinessCenterNameOnDate]
  (	
 	@idEmployee int,
	@date datetime
  )
  RETURNS nvarchar(64)
  AS
  BEGIN
    	DECLARE @businessCenter nvarchar(64)
		select @businessCenter = 
    	(select top 1 BusinessCenters.Name 
                from Groups 
                inner join dbo.GetEmployeeGroupTree(@idEmployee,null,@date) eg on Groups.ID = eg.ID 
                inner join BusinessCenters on Groups.IDCenter = BusinessCenters.id 
                WHERE IDCenter is not null order by Path DESC)
    	   		   
    	RETURN @businessCenter
  END
GO

  CREATE FUNCTION [dbo].[GetEmployeeBusinessNameCenter]
  (	
 	@idEmployee int
  )
  RETURNS nvarchar(64)
  AS
  BEGIN
    	DECLARE @businessCenter nvarchar(64)
		select @businessCenter = [dbo].[GetEmployeeBusinessCenterOnDate](@idEmployee, getdate())

		RETURN @businessCenter
  END
GO

 ALTER VIEW [dbo].[sysrovwAllEmployeeGroups]
  AS
  SELECT GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
  JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  , dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwCurrentEmployeeGroups.IDEmployee,sysrovwCurrentEmployeeGroups.BeginDate) As CostCenter
  FROM dbo.Groups        
  inner JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwCurrentEmployeeGroups.IDGroup
  UNION
  SELECT GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, AttControlled, AccControlled, 
  JobControlled, ExtControlled, RiskControlled, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName,(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany
  , dbo.GetEmployeeBusinessCenterNameOnDate(sysrovwFutureEmployeeGroups.IDEmployee,sysrovwFutureEmployeeGroups.BeginDate)  As CostCenter
  FROM dbo.Groups
  INNER JOIN dbo.sysrovwFutureEmployeeGroups ON dbo.Groups.ID = dbo.sysrovwFutureEmployeeGroups.IDGroup
GO

 
UPDATE dbo.sysroParameters SET Data='361' WHERE ID='DBVersion'
GO