CREATE OR ALTER PROCEDURE [dbo].[Report_visitantes_presentes]
@IDReportTask nvarchar(max) = '0'
AS
select distinct tmp.*, e.FullGroupName, e.EmployeeName, e.GroupName, e.Path, e.IDGroup from TMPVisitsLocation tmp
join sysrovwAllEmployeeGroups e ON tmp.IDEmployee = e.IDEmployee
where tmp.IDReportTask = @IDReportTask
RETURN NULL
GO

ALTER PROCEDURE [dbo].[Report_presentes]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '114,242',
@userField1 nvarchar(100) = '0',
@userField2 nvarchar(100) = '0',
@userField3 nvarchar(100) = '0',
@userField4 nvarchar(100) = '0'
AS
SELECT vc.IDEmployee, [DateTime],
(select Value from EmployeeUserFieldValues where IDEmployee = vc.IDEmployee AND FieldName = (Select FieldName from sysroUserFields where ID = @userField1)) As userField1,
(select Value from EmployeeUserFieldValues where IDEmployee = vc.IDEmployee AND FieldName = (Select FieldName from sysroUserFields where ID = @userField2)) As userField2,
(select Value from EmployeeUserFieldValues where IDEmployee = vc.IDEmployee AND FieldName = (Select FieldName from sysroUserFields where ID = @userField3)) As userField3,
(select Value from EmployeeUserFieldValues where IDEmployee = vc.IDEmployee AND FieldName = (Select FieldName from sysroUserFields where ID = @userField4)) As userField4,
eg.EmployeeName, eg.FullGroupName
FROM sysrovwCurrentEmployeesPresenceStatusPunches vc
inner join (select * from split(@IDEmployee,',')) tmp on vc.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
and (GETDATE() between poe.BeginDate and poe.EndDate
or GETDATE() between poe.BeginDate and poe.EndDate)
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join sysrovwAllEmployeeGroups eg on vc.IDEmployee = eg.IDEmployee
where (Status = 'IN' and DateTime >= DATEADD(dd,-2,GETDATE())) OR (Status = 'OUT' and DateTime >= DATEADD(MINUTE, -15, GETDATE()))
GROUP BY vc.IDEmployee, [DateTime], eg.EmployeeName, eg.FullGroupName
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='893' WHERE ID='DBVersion'
GO
