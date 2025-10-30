-- No borréis esta línea
CREATE OR ALTER FUNCTION dbo.sysfnEmployeesAnnualWorkPeriods (@IDEmployee INT)
RETURNS TABLE
AS
RETURN
(
WITH CTE_Fechas AS (
        SELECT IDEmployee, Begindate AS BeginPeriod, EndDate, BeginDate, IDLabAgree FROM EmployeeContracts  WITH (NOLOCK) WHERE IDEmployee= @IDEmployee
        UNION ALL
        SELECT IDEmployee, DATEADD(YEAR, 1, BeginPeriod), EndDate, BeginDate, IDLabAgree
        FROM CTE_Fechas 
        WHERE IDEmployee = @IDEmployee and DATEADD(YEAR, 1, BeginPeriod) <= CASE WHEN EndDate = '20790101' THEN  DATEFROMPARTS(YEAR(GETDATE()) +2, 12, 31) ELSE EndDate END
    )
	SELECT IDEmployee, BeginPeriod, EndPeriod, IDLabAgree 
	FROM (
			-- 05. En el caso que la fecha de fin de contrato sea anterior, limitar a la fecha de contrato
			SELECT IDEmployee, BeginPeriod, CASE WHEN EndPeriod> EndDate THEN EndDate ELSE EndPeriod END AS EndPeriod, IDLabAgree  FROM (
			-- 04. En el caso que la fecha de inicio de contrato sea 29/02, la fecha de fin de tramo siempre sera 28/02 del año siguiente
			SELECT IDEmployee, BeginPeriod, CASE WHEN dbo.Is0229(begindate) = 1 THEN DATEFROMPARTS(YEAR(BeginPeriod) +1, 2, 28) ELSE EndPeriod END AS EndPeriod, EndDate, IDLabAgree FROM (
			-- 03. El fin del tramo siempre sera un año mas que el inicio del tramo menos cuando la fecha de fin de contrato sea anterior
			SELECT IDEmployee, BeginPeriod, CASE WHEN DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) > EndDate THEN EndDate ELSE DATEADD(DAY,-1,DATEADD(YEAR, 1, BeginPeriod)) END AS EndPeriod, BeginDate, EndDate, IDLabAgree FROM (
			--02. Ademas si el tramo anterior es bisiesto y el inicio de contrato es 29/02, el actual siempre empezara en 01/03
			SELECT IsLeapYear, IDEmployee, CASE WHEN dbo.Is0229(BeginDate) = 1 AND dbo.IsLeapYear(YEAR(BeginPeriod)-1) = 1 THEN DATEFROMPARTS(YEAR(BeginPeriod),3,1) ELSE BeginPeriod END AS BeginPeriod, EndDate, BeginDate, IDLabAgree FROM 
			-- 01. En el caso que la fecha de inicio de contrato sea el 29/02, si es año bisiesto siempre empezara por 29/02 y si no empezará por 01/03, en cualquier otro caso la fecha de inicio sin tratamiento
			(SELECT dbo.IsLeapYear(YEAR(BeginPeriod)) AS IsLeapYear, IDEmployee, CASE WHEN dbo.IsLeapYear(YEAR(BeginPeriod)) = 1 AND dbo.Is0229(BeginDate) = 1 THEN DATEFROMPARTS(YEAR(beginperiod),2,29) ELSE CASE WHEN dbo.Is0229(BeginDate) = 1 THEN DATEFROMPARTS(YEAR(BeginPeriod),3,1) ELSE BeginPeriod END END AS BeginPeriod, EndDate, BeginDate, IDLabAgree
			FROM CTE_Fechas  WITH (NOLOCK) ) z ) w) t ) x
	) PeriodsTBL
)
GO

-- Vista saldos laborales con caducidad e inicio de disfrute, y con check de convenio
CREATE OR ALTER FUNCTION dbo.sysrofnEmployeeAnnualWorkPeriods_WithConcepts(@IDEmployee INT, @IDConcept INT)
RETURNS TABLE
AS
RETURN
(
    SELECT IDEmployee, BeginPeriod, EndPeriod, IDLabAgree, Concepts.ID IDConcept,
		   CASE WHEN HolidaysEnjoymentLabAgrees = '' OR (',' + HolidaysEnjoymentLabAgrees + ',') LIKE (',%' + CONVERT(NVARCHAR(10),IDLabAgree) + '%,') THEN
			    CASE WHEN Concepts.HolidaysEnjoymentType = 1 THEN DATEADD(DAY, HolidaysEnjoymentValue, BeginPeriod) 
				     WHEN Concepts.HolidaysEnjoymentType = 2 THEN DATEADD(MONTH, HolidaysEnjoymentValue, BeginPeriod) 
					 ELSE BeginPeriod 
					 END 
				ELSE BeginPeriod
				END AS BeginEnjoyDate,
		   CASE WHEN ISNULL(CONVERT(xml, Concepts.ExpiredHoursCriteria).value('(/roCollection/Item[@key=("LabAgreementsAffected")]/text())[1]', 'NVARCHAR(MAX)'),'') = '' 
		             OR (',' + ISNULL(CONVERT(xml, Concepts.ExpiredHoursCriteria).value('(/roCollection/Item[@key=("LabAgreementsAffected")]/text())[1]', 'NVARCHAR(MAX)'),'') + ',') LIKE (',%' + CONVERT(NVARCHAR(10),IDLabAgree) + '%,') THEN
			    CASE CONVERT(xml, Concepts.ExpiredHoursCriteria).value('(/roCollection/Item[@key=("ExpiredHoursType")]/text())[1]', 'INT') 
					 WHEN 0 THEN CONVERT(SMALLDATETIME,'20790101',120)
					 WHEN 1 THEN DATEADD(DAY, CONVERT(xml, Concepts.ExpiredHoursCriteria).value('(/roCollection/Item[@key=("Value")]/text())[1]', 'INT'), BeginPeriod)
					 WHEN 2 THEN DATEADD(MONTH, CONVERT(xml, Concepts.ExpiredHoursCriteria).value('(/roCollection/Item[@key=("Value")]/text())[1]', 'INT'), BeginPeriod) 
					 END
				ELSE CONVERT(SMALLDATETIME,'20790101',120)
				END AS ExpirationDate
	FROM dbo.sysfnEmployeesAnnualWorkPeriods(@IDEmployee) PeriodsTBL
	CROSS JOIN Concepts WHERE Concepts.DefaultQuery = 'L' AND (Concepts.ID = @IDConcept OR @IDConcept = 0)
)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='858' WHERE ID='DBVersion'
GO
