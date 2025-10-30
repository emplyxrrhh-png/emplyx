-- No borréis esta línea
ALTER VIEW [dbo].[sysrovwEmployeesAnnualWorkPeriods]	
AS
WITH CTE_Fechas AS (
        SELECT IDEmployee, Begindate AS BeginPeriod, EndDate, BeginDate FROM EmployeeContracts  WITH (NOLOCK)
        UNION ALL
        SELECT IDEmployee, DATEADD(YEAR, 1, BeginPeriod), EndDate, BeginDate
        FROM CTE_Fechas 
        WHERE DATEADD(YEAR, 1, BeginPeriod) <= CASE WHEN EndDate = '20790101' THEN  DATEFROMPARTS(year(getdate())+2,12,31) ELSE EndDate END
    )
	
	-- 05. En el caso que la fecha de fin de contrato sea anterior, limitar a la fecha de contrato
	select IDEmployee, BeginPeriod, case when EndPeriod> EndDate then endDate else EndPeriod end  as EndPeriod  from (
	-- 04. En el caso que la fecha de inicio de contrato sea 29/02, la fecha de fin de tramo siempre sera 28/02 del año siguiente
	select IDEmployee, BeginPeriod, case when dbo.Is0229(begindate)=1 then DATEFROMPARTS(year(beginperiod)+1,2,28) else EndPeriod end  as EndPeriod, endDate  from (
	-- 03. El fin del tramo siempre sera un año mas que el inicio del tramo menos cuando la fecha de fin de contrato sea anterior
	select IDEmployee, BeginPeriod, CASE WHEN DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) > EndDate THEN EndDate ELSE DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) END as EndPeriod, BeginDate, EndDate from (
    --02. Ademas si el tramo anterior es bisiesto y el inicio de contrato es 29/02, el actual siempre empezara en 01/03
	select IsLeapYear, idemployee, case when dbo.Is0229(BeginDate) = 1 and dbo.IsLeapYear(year(BeginPeriod)-1) = 1 then DATEFROMPARTS(year(beginperiod),3,1) else BeginPeriod end as BeginPeriod, enddate, BeginDate  from 
	-- 01. En el caso que la fecha de inicio de contrato sea el 29/02, si es año bisiesto siempre empezara por 29/02 y si no empezará por 01/03, en cualquier otro caso la fecha de inicio sin tratamiento
	(SELECT dbo.IsLeapYear(year(BeginPeriod)) as IsLeapYear, IDEmployee, case when dbo.IsLeapYear(year(BeginPeriod)) = 1 and dbo.Is0229(BeginDate) =1   then DATEFROMPARTS(year(beginperiod),2,29) else case when dbo.Is0229(BeginDate) =1 then DATEFROMPARTS(year(beginperiod),3,1) else BeginPeriod end end  as beginPeriod, EndDate, BeginDate
    FROM CTE_Fechas  WITH (NOLOCK) ) z ) w) t ) x
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='705' WHERE ID='DBVersion'
GO
