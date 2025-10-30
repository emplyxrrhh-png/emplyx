alter VIEW [dbo].[sysrovwAllEmployeeGroups]    
  AS    
  SELECT        geg.GroupName, sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate as BeginDate, geg.EndDate    
  , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled    
  , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,     
      sysrovwCurrentEmployeeGroups.isTransfer,'ERROR' As CostCenter    
  FROM            dbo.sysrovwGetEmployeeGroup geg    
  INNER JOIN dbo.sysrovwCurrentEmployeeGroups    
  ON geg.IDGroup = dbo.sysrovwCurrentEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwCurrentEmployeeGroups.IDEmployee and DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0) between geg.BeginDate and geg.EndDate    
  UNION    
  SELECT        geg.GroupName, sysrovwFutureEmployeeGroups.Path, sysrovwFutureEmployeeGroups.SecurityFlags, geg.IDEmployee, geg.IDGroup, EmployeeName, geg.RealBeginDate, geg.EndDate    
  , CurrentEmployee, AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled    
  , convert(nvarchar(300),geg.FullGroupName) as FullGroupName, geg.IDCompany,     
      sysrovwFutureEmployeeGroups.isTransfer,'ERROR' As CostCenter    
  FROM            dbo.sysrovwGetEmployeeGroup geg    
  INNER JOIN dbo.sysrovwFutureEmployeeGroups     
   ON geg.IDgroup = dbo.sysrovwFutureEmployeeGroups.IDGroup and geg.IDEmployee=dbo.sysrovwFutureEmployeeGroups.IDEmployee and geg.RealBeginDate > DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0) 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='605' WHERE ID='DBVersion'
GO
