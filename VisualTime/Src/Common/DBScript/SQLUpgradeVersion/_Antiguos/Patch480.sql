ALTER PROCEDURE [dbo].[Analytics_CostCenters]  
       @initialDate smalldatetime,  
       @endDate smalldatetime,  
       @idpassport int,  
       @userFieldsFilter nvarchar(max),  
    @businessCenterFilter nvarchar(max)  
       AS  
    DECLARE @businessCenterIDs Table(idBusinessCenter int)  
     declare @pinitialDate smalldatetime = @initialDate,    
      @pendDate smalldatetime = @endDate,    
      @pidpassport int = @idpassport,    
      @pbusinessCenterFilter nvarchar(max) = @businessCenterFilter,    
      @puserFieldsFilter nvarchar(max) = @userFieldsFilter    
    insert into @businessCenterIDs select * from dbo.SplitToInt(@pbusinessCenterFilter,',');  
        SELECT     CONVERT(varchar(10), dbo.DailyCauses.IDEmployee, 120) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar)   
                              + '-' + dbo.EmployeeContracts.IDContract + CONVERT(varchar(10), dbo.DailyCauses.Date, 120) AS KeyView, dbo.DailyCauses.Date,  
                              dbo.Causes.Name AS CauseName, convert(numeric(18,6), isnull(dbo.Causes.CostFactor, 0)) as CostFactor,  isnull(dbo.BusinessCenters.ID, 0) as IDCenter,   
            isnull(dbo.BusinessCenters.Name, '') AS CenterName,  isnull(dbo.BusinessCenters.Field1, '') AS Field1,  isnull(dbo.BusinessCenters.Field2, '') AS Field2 ,  isnull(dbo.BusinessCenters.Field3, '') AS Field3,    
            isnull(dbo.BusinessCenters.Field4, '') AS Field4, isnull(dbo.BusinessCenters.Field5, '') AS Field5, dbo.DailyCauses.Value AS Value, YEAR(dbo.DailyCauses.Date) AS Año,   
                              (DATEPART(dw, dbo.DailyCauses.Date) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.DailyCauses.Date) AS WeekOfYear, DATEPART(dy,   
                              dbo.DailyCauses.Date) AS DayOfYear, dbo.DailyCauses.DefaultCenter, dbo.DailyCauses.ManualCenter ,                              
            dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'),DailyCauses.date ) as Cost,   
            dbo.GetValueFromEmployeeUserFieldValues(dbo.DailyCauses.IDEmployee, (SELECT Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'), DailyCauses.date) as PVP  
        FROM         dbo.sysroEmployeeGroups with (nolock)   
                              INNER JOIN dbo.Causes with (nolock)   
                              INNER JOIN dbo.DailyCauses with (nolock) ON dbo.Causes.ID = dbo.DailyCauses.IDCause  ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.DailyCauses.Date between dbo.sysroEmployeeGroups.BeginDate AND dbo
 .sysroEmployeeGroups.EndDate   
            INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.DailyCauses.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyCauses.Date <= dbo.EmployeeContracts.EndDate   
            INNER JOIN dbo.Groups with (nolock) ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID   
            LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.DailyCauses.IDEmployee = dbo.Employees.ID    
            LEFT OUTER JOIN dbo.BusinessCenters with (nolock) ON isnull(dbo.DailyCauses.IDCenter,0) = dbo.BusinessCenters.ID   
         where     isnull(dbo.DailyCauses.IDCenter,0) in (select idBusinessCenter from @businessCenterIDs)   
		 and isnull(dbo.DailyCauses.IDCenter,0) in (select idcenter from sysroPassports_Centers where IDPassport = (select idparentpassport from sysroPassports where id = @pidpassport))

      and dbo.dailycauses.date between @pinitialDate and @pendDate 
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='480' WHERE ID='DBVersion'
GO

