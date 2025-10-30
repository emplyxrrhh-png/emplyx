ALTER PROCEDURE [dbo].[Report_auditoria_fichajes]
	@idPassport nvarchar(100) = '1',
	@startDate datetime2 = '20210301',
	@endDate datetime2 = '20210301',
	@IDEmployee nvarchar(max) = '114,242'
AS
select p.action, p.DateTime, p.TimeStamp as AuditPunchDate, p.IDPassport, sp.Name as UserName, p.actualType, p.IDTerminal, t.Description as terminalDescription, t.Type as terminalType, eg.EmployeeName, p.IDEmployee, eg.FullGroupName, pc.IDPunch as punchCapture, p.isNotReliable, p.inTelecommute, (SELECT TOP 1 Value FROM EmployeeUserFieldValues  
WHERE EmployeeUserFieldValues.IDEmployee = 
p.IDEmployee and FieldName = 'NIF' ORDER BY EmployeeUserFieldValues.Date DESC) AS Dni, (select count(*) from requests r where r.RequestType in (2,15) and r.idemployee = p.idemployee and r.date1 = p.datetime) as Request 
from punches p 
inner join (select * from split(@IDEmployee,',')) tmp on p.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
And poe.IDEmployee = p.IDEmployee
and CAST(p.DateTime as date) between poe.BeginDate and poe.EndDate
and poe.IdFeature = 1 and poe.FeaturePermission > 1
left join Terminals t on p.IDTerminal = t.ID
left join PunchesCaptures pc on p.id = pc.IDPunch
join sysrovwAllEmployeeGroups eg on p.IDEmployee = eg.IDEmployee 
left join sysroPassports sp on p.IDPassport = sp.ID
where (p.type between 1 and 3 or p.Type = 7)
and CAST(p.DateTime as date) between CAST(@startDate as date) and CAST(@endDate as date) 
order by p.DateTime
RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='986' WHERE ID='DBVersion'
GO
