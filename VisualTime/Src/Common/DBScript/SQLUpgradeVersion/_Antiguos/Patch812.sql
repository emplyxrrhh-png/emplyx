-- No borréis esta línea
DROP PROCEDURE IF EXISTS [dbo].[Report_saldos_con_detalle]
GO
CREATE PROCEDURE [dbo].[Report_saldos_con_detalle]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@IDConcept nvarchar(max) = '0',
	@FechaInicio date = '20210301',
	@FechaFin date = '20210301'
AS
select emp.ID AS EmployeeID, emp.Name As EmployeeName, ec.IDContract as Contract, g.Id as IDGroup, g.Name as GroupName, g.FullGroupName as Ruta, ds.Date, cs.Name as ConceptName, cs.ID as ConceptID, ds.Value from DailyAccruals ds 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = ds.IDEmployee And cast(ds.Date as date) between poe.BeginDate And poe.EndDate 
inner join Employees emp on emp.ID = ds.IDEmployee 
inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate 
inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate 
inner join Groups g on eg.IDGroup = g.ID
left join Concepts cs on ds.IDConcept = cs.ID where cast(ds.Date as date) between cast(@FechaInicio as date) and cast(@FechaFin as date) and ds.IDConcept IN (select * from split(@IDConcept,','))
RETURN NULL 
GO


DROP PROCEDURE IF EXISTS [dbo].[Report_saldos_con_detalle_total]
GO
CREATE PROCEDURE [dbo].[Report_saldos_con_detalle_total]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@IDConcept nvarchar(max) = '0',
	@ContractID nvarchar(max) = '0',
	@GroupId smallint = '0',
	@FechaInicio date = '20210301',
	@FechaFin date = '20210301'
AS
select cs.Name AS ConceptName,cs.ID AS ConceptID, sum(ds.Value) AS ConceptValue, sub.rownum 
	from DailyAccruals ds
	inner join (select * from splitMAX(@IDEmployee,',')) tmp on ds.IDEmployee = tmp.Value
	inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = ds.IDEmployee And cast(ds.Date as date) between poe.BeginDate And poe.EndDate 
	JOIN (
        SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS rownum
        from split(@IDConcept,',')
    ) AS sub
    ON ds.IDConcept = sub.Value
	inner join Employees emp on emp.ID = ds.IDEmployee
	inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate
	inner join EmployeeGroups eg on eg.IDEmployee = ds.IDEmployee and ds.Date between eg.BeginDate and eg.EndDate
	inner join Groups g on eg.IDGroup = g.ID
	left join Concepts cs on ds.IDConcept = cs.ID
	where ds.Date between @FechaInicio and @FechaFin and ec.IDContract = @ContractID and eg.IDGroup = @GroupId  and ds.IDConcept in (select * from split(@IDConcept,',')) group by cs.name, cs.ID, sub.rownum
	RETURN NULL 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='812' WHERE ID='DBVersion'
GO
