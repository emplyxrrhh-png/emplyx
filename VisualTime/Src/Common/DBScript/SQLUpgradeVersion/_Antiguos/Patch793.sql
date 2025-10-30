ALTER VIEW dbo.sysrosubvwCurrentEmployeePeriod  
AS  
SELECT IDEmployee  
FROM dbo.EmployeeContracts  
WHERE BeginDate <= CONVERT(date, GETDATE()) AND EndDate >= CONVERT(date, GETDATE())
GO



DROP VIEW IF EXISTS sysrovwEmployees_AllMobilities
GO
CREATE VIEW sysrovwEmployees_AllMobilities
AS
 select 
	 eg.idemployee,
	 eg.idgroup,
	 eg.begindate, eg.enddate, groups.Path, case when cep.IDEmployee is null then 0 else 1 end as CurrentEmployee
 from EmployeeGroups eg
     inner join groups on groups.ID = eg.IDGroup
	 left join sysrosubvwCurrentEmployeePeriod cep on cep.IDEmployee = eg.IDEmployee
GO



DROP VIEW IF EXISTS sysrovwEmployees_CurrentAndFutureMobilities
GO
CREATE VIEW sysrovwEmployees_CurrentAndFutureMobilities
AS
 select * from sysrovwEmployees_AllMobilities geg where Convert(Date, GETDATE()) between geg.BeginDate and geg.EndDate or geg.BeginDate > Convert(Date, GETDATE())
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='793' WHERE ID='DBVersion'
GO
