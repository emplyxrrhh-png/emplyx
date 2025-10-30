ALTER PROCEDURE [dbo].[ObtainEmployeeIDsFromFilter]    
     @employeeFilter nvarchar(max),    
     @initialDate smalldatetime,    
     @endDate smalldatetime    
    AS     
    BEGIN  
      declare @pemployeeFilter nvarchar(max) = @employeeFilter    
      declare @pinitialDate smalldatetime   = @initialDate   
      declare @pendDate smalldatetime  = @endDate   
     
	  DECLARE @SQLString nvarchar(MAX);    
      SET @SQLString = 'SELECT DISTINCT SYSROEMPLOYEEGROUPS.IDEmployee FROM (SELECT G.IDEMPLOYEE,
CASE WHEN G.BeginDate <= EC.BeginDate THEN EC.BeginDate ELSE G.BeginDate END AS BEGINDATE ,
CASE WHEN G.ENDDATE >= EC.ENDDATE THEN EC.ENDDATE ELSE G.ENDDate END AS ENDDATE, IDGROUP FROM EmployeeGroups AS G
INNER JOIN EMPLOYEECONTRACTS AS EC ON EC.IDEmployee = G.IDEMPLOYEE) AS SYSROEMPLOYEEGROUPS '
	  SET @SQLString = @SQLString + ' WHERE (' + @pemployeeFilter + ') AND '
      SET @SQLString = @SQLString + '((''' + CONVERT(VARCHAR(10), @pinitialDate, 112) + ''' <= SYSROEMPLOYEEGROUPS.EndDate) AND '
	  SET @SQLString = @SQLString + ' (''' + CONVERT(VARCHAR(10), @pendDate, 112) + ''' >= SYSROEMPLOYEEGROUPS.BeginDate))'    
      SET @SQLString = @SQLString + 'AND SYSROEMPLOYEEGROUPS.BEGINDATE <= SYSROEMPLOYEEGROUPS.ENDDATE ORDER BY IDEmployee'    
      
	  exec sp_executesql @SQLString
    end
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='846' WHERE ID='DBVersion'

GO
